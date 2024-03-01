using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Drawing;
using EdiEngine.Runtime;
using EdiEngine;
using JUST;
using eSyncMate.DB.Entities;
using System.Reflection;
using eSyncMate.Processor.Models;
using eSyncMate.Processor.Managers;
using System.Data;
using eSyncMate.DB;
using Nancy;
using Hangfire;
using Microsoft.AspNetCore.Authorization;

namespace eSyncMate.Processor.Controllers
{
    [ApiController]
    [Route("EDIProcessor/api/v1/orders")]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public OrdersController(ILogger<OrdersController> logger, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;

            //_recurringJobManager.AddOrUpdate("syncerror", () => OrderManager.ProcessSyncJob(CommonUtils.ConnectionString), Cron.Hourly);
            //_recurringJobManager.AddOrUpdate("synced", () => OrderManager.ProcessASNJob(this._logger, CommonUtils.ConnectionString), Cron.Hourly);
            //_recurringJobManager.AddOrUpdate("markasn", () => OrderManager.MarkASNJob(CommonUtils.ConnectionString), Cron.Hourly);
            //_recurringJobManager.AddOrUpdate("invoice", () => OrderManager.CreateInvoiceJob(CommonUtils.ConnectionString), Cron.Hourly);
            //_recurringJobManager.AddOrUpdate("810", () => OrderManager.Process810Job(this._logger, CommonUtils.ConnectionString), Cron.Hourly);
        }

        [HttpPost]
        [Route("process850")]
        public async Task<OrderResponseModel> ProcessEDIFile(IFormFile file)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            string l_TransformationMap = string.Empty;
            string l_DBFieldsMap = string.Empty;
            string l_EDIData = string.Empty;
            int l_SystemUser = 1;

            OrderResponseModel l_Response = new OrderResponseModel();
            Customers l_Customer = new Customers();
            EdiDataReader r = new EdiDataReader();
            InboundEDI l_EDI = new InboundEDI();

            l_Response.Code = (int)ResponseCodes.Error;

            try
            {
                if (file.Length <= 0)
                {
                    this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Please provide a eSyncMate data file.");

                    l_Response.Message = "Please provide a eSyncMate data file";
                    return l_Response;
                }

                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    l_EDIData = await reader.ReadToEndAsync();
                }

                l_EDI.UseConnection(CommonUtils.ConnectionString);

                l_EDI.Type = "850";
                l_EDI.Status = "NEW";
                l_EDI.Data = l_EDIData;
                l_EDI.CreatedBy = l_SystemUser;
                l_EDI.CreatedDate = DateTime.Now;

                l_EDI.SaveNew();

                EdiBatch b = r.FromString(l_EDIData);

                l_Customer.UseConnection(CommonUtils.ConnectionString);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring processing eSyncMate data.");
                foreach (EdiInterchange i in b.Interchanges)
                {
                    if (!l_Customer.GetObject("ISACustomerID", i.ISA.Content[5].Val.ToString().Trim()).IsSuccess)
                    {
                        this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - The party [{i.ISA.Content[5].Val.ToString().Trim()}] is not registered.");

                        l_EDI.Status = "ERROR";
                        l_EDI.Modify();

                        l_Response.Message = $"The party [{i.ISA.Content[5].Val.ToString().Trim()}] is not registered";
                        return l_Response;
                    }

                    var l_Map = l_Customer.Maps.Where(p => p.MapTypeName == "850 Transformation");

                    if (l_Map != null)
                    {
                        l_TransformationMap = l_Map.FirstOrDefault<CustomerMaps>()?.Map;
                    }

                    l_Map = l_Customer.Maps.Where(p => p.MapTypeName == "850 DB Fields");

                    if (l_Map != null)
                    {
                        l_DBFieldsMap = l_Map.FirstOrDefault<CustomerMaps>()?.Map;
                    }

                    if (string.IsNullOrEmpty(l_TransformationMap) || string.IsNullOrEmpty(l_DBFieldsMap))
                    {
                        this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Required maps for 850 processing are missing for [{l_Customer.Name}].");

                        l_EDI.Status = "ERROR";
                        l_EDI.Modify();

                        l_Response.Message = $"Required maps for 850 processing are missing for [{l_Customer.Name}]";
                        return l_Response;
                    }

                    this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - eSyncMate Customer with required Maps found.");

                    InboundEDIInfo l_EDIInfo = new InboundEDIInfo();

                    l_EDIInfo.InboundEDIId = l_EDI.Id;
                    l_EDIInfo.ISASenderQual = i.ISA.Content[4].Val.ToString().Trim();
                    l_EDIInfo.ISASenderId = i.ISA.Content[5].Val.ToString().Trim();
                    l_EDIInfo.ISAReceiverQual = i.ISA.Content[6].Val.ToString().Trim();
                    l_EDIInfo.ISAReceiverId = i.ISA.Content[7].Val.ToString().Trim();
                    l_EDIInfo.ISAEdiVersion = i.ISA.Content[11].ToString().Trim();
                    l_EDIInfo.ISAUsageIndicator = i.ISA.Content[14].ToString().Trim();
                    l_EDIInfo.ISAControlNumber = i.ISA.Content[12].ToString().Trim();
                    l_EDIInfo.SegmentSeparator = i.SegmentSeparator;
                    l_EDIInfo.ElementSeparator = i.ElementSeparator;
                    l_EDIInfo.CreatedBy = l_SystemUser;
                    l_EDIInfo.CreatedDate = DateTime.Now;

                    l_EDIInfo.UseConnection(CommonUtils.ConnectionString);

                    l_Response.Orders = new List<OrderModel>();

                    foreach (EdiGroup g in i.Groups)
                    {
                        l_EDIInfo.GSSenderId = g.GS.Content[1].ToString().Trim();
                        l_EDIInfo.GSReceiverId = g.GS.Content[2].ToString().Trim();
                        l_EDIInfo.GSControlNumber = g.GS.Content[5].ToString().Trim();
                        l_EDIInfo.GSEdiVersion = g.GS.Content[7].ToString().Trim();

                        l_EDIInfo.SaveNew();

                        foreach (EdiTrans t in g.Transactions)
                        {
                            this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Processing Maps.");
                            OrderTransformationResponseModel l_OrderData = OrderManager.ParseOrder(l_Customer, t, l_TransformationMap, l_DBFieldsMap);

                            if (l_OrderData.Code != (int)ResponseCodes.Success)
                            {
                                this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {l_OrderData.Message}");

                                l_Response.Code = l_OrderData.Code;
                                l_Response.Message = l_OrderData.Message;

                                return l_Response;
                            }

                            l_OrderData.EDI = l_EDIData;
                            l_OrderData.SystemUser = l_SystemUser;

                            this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Maps executed successfully.");
                            this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Processing DB Operations.");

                            OrderSaveResponseModel l_OrderResponse = OrderManager.SaveOrder(l_EDI, l_Customer, l_OrderData);

                            if (l_OrderResponse.Code != (int)ResponseCodes.Success)
                            {
                                this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {l_OrderResponse.Message}");

                                l_Response.Code = l_OrderResponse.Code;
                                l_Response.Message = l_OrderResponse.Message;

                                return l_Response;
                            }

                            this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - DB Operations completed successfully.");
                            this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Processing Third Party Sync.");

                            OrderSyncResponseModel l_SyncResponse = await OrderManager.SyncOrder(l_Customer, l_OrderResponse.OrderId);

                            if (l_SyncResponse.Code != (int)ResponseCodes.Success)
                            {
                                this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {l_SyncResponse.Message}");
                            }
                            else
                            {
                                l_Response.Orders.Add(new OrderModel { OrderId = l_OrderResponse.OrderId });
                                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Third Party Sync completed successfully.");
                            }
                        }
                    }
                }

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Order processed successfully!";

                l_EDI.Status = "PROCESSED";
                l_EDI.Modify();

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - eSyncMate data processing completed successfully.");
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;

                l_EDI.Status = "EXCEPTION";
                l_EDI.Modify();

                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }

            return l_Response;
        }

        [HttpPost]
        [Route("syncOrder")]
        public async Task<OrderResponseModel> SyncOrder(int OrderId)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();

            OrderResponseModel l_Response = new OrderResponseModel();
            Orders l_Order = new Orders();

            l_Response.Code = (int)ResponseCodes.Error;

            try
            {
                l_Order.UseConnection(CommonUtils.ConnectionString);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Processing Third Party Sync.");

                OrderSyncResponseModel l_SyncResponse = await OrderManager.SyncOrder(OrderId);

                if (l_Response.Code != (int)ResponseCodes.Success)
                {
                    this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {l_SyncResponse.Message}");

                    l_Response.Code = l_SyncResponse.Code;
                    l_Response.Message = l_SyncResponse.Message;

                    return l_Response;
                }

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Third Party Sync completed successfully.");

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Order processed successfully!";
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }

            return l_Response;
        }

        [HttpPost]
        [Route("syncOrderStore")]
        public async Task<OrderStoreResponseModel> SyncOrderStore(int OrderStoreId)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();

            OrderStoreResponseModel l_Response = new OrderStoreResponseModel();
            OrderStores l_OrderStore = new OrderStores();

            l_Response.Code = (int)ResponseCodes.Error;

            try
            {
                l_OrderStore.UseConnection(CommonUtils.ConnectionString);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Processing Third Party Sync.");

                OrderStoreSyncResponseModel l_SyncResponse = await OrderManager.SyncOrderStore(OrderStoreId);

                if (l_Response.Code != (int)ResponseCodes.Success)
                {
                    this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {l_SyncResponse.Message}");

                    l_Response.Code = l_SyncResponse.Code;
                    l_Response.Message = l_SyncResponse.Message;

                    return l_Response;
                }

                //l_OrderStore.GetObject(OrderStoreId);
                //l_OrderStore.UpdateOrderStatus(l_OrderStore.OrderId);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Third Party Sync completed successfully.");

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Order processed successfully!";
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }

            return l_Response;
        }

        [HttpGet]
        [Route("getOrderStores")]
        public async Task<GetOrderStoresResponseModel> GetOrderStores(int OrderId)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            GetOrderStoresResponseModel l_Response = new GetOrderStoresResponseModel();
            DataTable l_Data = new DataTable();

            try
            {
                string l_Criteria = string.Empty;
                OrderStores l_Order = new OrderStores();

                l_Response.Code = (int)ResponseCodes.Error;

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Building search criteria.");

                l_Order.UseConnection(CommonUtils.ConnectionString);

                if (OrderId > 0)
                {
                    l_Criteria = $"OrderId = {OrderId}";
                }

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Search criteria ready ({l_Criteria}).");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring order stores search.");

                l_Order.GetList(l_Criteria, string.Empty, ref l_Data, "Id ASC");

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Order Stores searched {{{l_Data.Rows.Count}}}.");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Populating order stores.");

                l_Response.OrderStores = new List<OrderStoreDataModel>();
                foreach (DataRow l_Row in l_Data.Rows)
                {
                    OrderStoreDataModel l_OrderRow = new OrderStoreDataModel();

                    DBEntity.PopulateObjectFromRow(l_OrderRow, l_Data, l_Row);

                    l_Response.OrderStores.Add(l_OrderRow);
                }

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Order Stores fetched successfully!";

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Order Stores are ready.");
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }
            finally
            {
                l_Data.Dispose();
            }

            return l_Response;
        }

        [HttpPost]
        [Route("process855")]
        public async Task<OrderResponseModel> Generate855EDI(int OrderId)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();

            OrderResponseModel l_Response = new OrderResponseModel();
            OrderObjectModel l_Order = new OrderObjectModel();
            string l_TransformationMap = string.Empty;

            l_Response.Code = (int)ResponseCodes.Error;

            try
            {
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Processing 855 eSyncMate for Order [{OrderId}].");

                ResponseModel l_SyncResponse = await OrderManager.Process855(this._logger, OrderId);

                if (l_SyncResponse.Code != (int)ResponseCodes.Success)
                {
                    this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {l_SyncResponse.Message}");

                    l_Response.Code = l_SyncResponse.Code;
                    l_Response.Message = l_SyncResponse.Message;

                    return l_Response;
                }

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Generate 855 eSyncMate completed successfully.");

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "855 eSyncMate completed successfully!";
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }

            return l_Response;
        }

        [HttpPost]
        [Route("process856")]
        public async Task<OrderResponseModel> Process856ForOrder(int OrderId)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();

            OrderResponseModel l_Response = new OrderResponseModel();

            l_Response.Code = (int)ResponseCodes.Error;

            try
            {
                l_Response = await OrderManager.ProcessASN(this._logger, OrderId);
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }

            return l_Response;
        }

        [HttpPost]
        [Route("markFor856")]
        public async Task<OrderResponseModel> MarkForASN(int OrderId)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();

            OrderResponseModel l_Response = new OrderResponseModel();
            OrderObjectModel? l_Order = null;

            l_Response.Code = (int)ResponseCodes.Error;

            try
            {
                l_Order = await OrderManager.GetOrder(OrderId);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Processing Mark for ASN for Order [{OrderId}].");

                ResponseModel l_SyncResponse = await OrderManager.MarkOrderForASN(l_Order.Customer, l_Order.Order);

                if (l_SyncResponse.Code != (int)ResponseCodes.Success)
                {
                    this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {l_SyncResponse.Message}");

                    l_Response.Code = l_SyncResponse.Code;
                    l_Response.Message = l_SyncResponse.Message;

                    return l_Response;
                }

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Mark for ASN completed successfully.");

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Order marked for ASN successfully!";
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }

            return l_Response;
        }

        [HttpPost]
        [Route("createInvoice")]
        public async Task<OrderResponseModel> CreateInvoice(int OrderId)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();

            OrderResponseModel l_Response = new OrderResponseModel();
            OrderObjectModel l_Order = new OrderObjectModel();
            string l_TransformationMap = string.Empty;

            l_Response.Code = (int)ResponseCodes.Error;

            try
            {
                l_Order = await OrderManager.GetOrder(OrderId);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Processing Create Invoice for Order [{OrderId}].");

                ResponseModel l_SyncResponse = await OrderManager.CreateInvoice(l_Order.Customer, l_Order.Order);

                if (l_SyncResponse.Code != (int)ResponseCodes.Success)
                {
                    this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {l_SyncResponse.Message}");

                    l_Response.Code = l_SyncResponse.Code;
                    l_Response.Message = l_SyncResponse.Message;

                    return l_Response;
                }

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Create Invoice completed successfully.");

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Invoice created successfully!";
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }

            return l_Response;
        }

        [HttpPost]
        [Route("process810")]
        public async Task<OrderResponseModel> Generate810EDI(int OrderId)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();

            OrderResponseModel l_Response = new OrderResponseModel();
            OrderObjectModel l_Order = new OrderObjectModel();
            string l_TransformationMap = string.Empty;

            l_Response.Code = (int)ResponseCodes.Error;

            try
            {
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Processing 810 eSyncMate for Order [{OrderId}].");

                ResponseModel l_SyncResponse = await OrderManager.Process810(this._logger, OrderId);

                if (l_SyncResponse.Code != (int)ResponseCodes.Success)
                {
                    this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {l_SyncResponse.Message}");

                    l_Response.Code = l_SyncResponse.Code;
                    l_Response.Message = l_SyncResponse.Message;

                    return l_Response;
                }

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Generate 810 eSyncMate completed successfully.");

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "810 eSyncMate completed successfully!";
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }

            return l_Response;
        }

        [HttpGet]
        [Route("getOrders/{OrderId}/{FromDate}/{ToDate}/{OrderNumber}/{status}")]
        public async Task<GetOrdersResponseModel> GetOrders(int OrderId = 0, string FromDate = "", string ToDate = "", string OrderNumber = "", string Status = "")
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            GetOrdersResponseModel l_Response = new GetOrdersResponseModel();
            DataTable l_Data = new DataTable();

            try
            {
                string l_Criteria = string.Empty;
                Orders l_Order = new Orders();

                if (FromDate == "1999-01-01")
                {
                    FromDate = string.Empty;
                }
                if (ToDate == "1999-01-01")
                {
                    ToDate = string.Empty;
                }
                if (OrderNumber == "EMPTY")
                {
                    OrderNumber = string.Empty;
                }
                if (Status == "Select Status")
                {
                    Status = string.Empty;
                }

                l_Response.Code = (int)ResponseCodes.Error;

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Building search criteria.");

                l_Order.UseConnection(CommonUtils.ConnectionString);

                l_Criteria = $"Status <> 'DELETED'";

                if (OrderId > 0)
                {
                    l_Criteria += $" AND Id = {OrderId}";
                }

                if (!string.IsNullOrEmpty(OrderNumber))
                {
                    l_Criteria += $" AND OrderNumber = '{OrderNumber}'";
                }

                if (!string.IsNullOrEmpty(Status))
                {
                    l_Criteria += $" AND Status = '{Status}'";
                }

                if (!string.IsNullOrEmpty(FromDate))
                {
                    l_Criteria += $" AND OrderDate >= '{FromDate}'";
                }

                if (!string.IsNullOrEmpty(ToDate))
                {
                    l_Criteria += $" AND OrderDate <= '{ToDate}'";
                }

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Search criteria ready ({l_Criteria}).");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring order search.");

                l_Order.GetViewList(l_Criteria, string.Empty, ref l_Data, "Id DESC");

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Orders searched {{{l_Data.Rows.Count}}}.");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Populating orders.");

                l_Response.Orders = new List<OrderDataModel>();
                foreach (DataRow l_Row in l_Data.Rows)
                {
                    OrderDataModel l_OrderRow = new OrderDataModel();

                    DBEntity.PopulateObjectFromRow(l_OrderRow, l_Data, l_Row);

                    l_Response.Orders.Add(l_OrderRow);
                }

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Orders fetched successfully!";

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Orders are ready.");
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }
            finally
            {
                l_Data.Dispose();
            }

            return l_Response;
        }

        [HttpGet]
        [Route("getOrderFiles/{OrderId}")]
        public async Task<GetOrderFilesResponseModel> GetOrderFiles(int OrderId)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            GetOrderFilesResponseModel l_Response = new GetOrderFilesResponseModel();
            DataTable l_Data = new DataTable();

            try
            {
                string l_Criteria = string.Empty;
                OrderData l_OrderData = new OrderData();

                l_Response.Code = (int)ResponseCodes.Error;

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Building search criteria.");

                l_OrderData.UseConnection(CommonUtils.ConnectionString);

                l_Criteria = $"OrderId = {OrderId}";

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Search criteria ready ({l_Criteria}).");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring order files search.");

                l_OrderData.GetViewList(l_Criteria, string.Empty, ref l_Data, "Id");

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Order files searched {{{l_Data.Rows.Count}}}.");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Populating order files.");

                l_Response.Files = new List<OrderFileModel>();
                foreach (DataRow l_Row in l_Data.Rows)
                {
                    OrderFileModel l_OrderFile = new OrderFileModel();

                    DBEntity.PopulateObjectFromRow(l_OrderFile, l_Data, l_Row);

                    l_Response.Files.Add(l_OrderFile);
                }

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Order Files fetched successfully!";

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Order Files are ready.");
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }
            finally
            {
                l_Data.Dispose();
            }

            return l_Response;
        }

        [HttpPost]
        [Route("process856Store")]
        public async Task<OrderResponseModel> Process856ForStoreOrders(IFormFile file, int OrderId)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();

            OrderResponseModel l_Response = new OrderResponseModel();
            string l_CSVData = string.Empty;

            l_Response.Code = (int)ResponseCodes.Error;

            try
            {
                if (file.Length <= 0)
                {
                    this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Please provide a csv data file.");

                    l_Response.Message = "Please provide a csv data file";
                    return l_Response;
                }

                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    l_CSVData = await reader.ReadToEndAsync();
                }

                string[] columnNames = { "SONo", "CustomerPO", "Dept", "VendorStyle", "UPC", "StoreNo", "Qty", "DC", "TrackingNo", "ShipDate", "OrderDate", "TotalCarton", "Carrier", "VendorID", "BOL" };
                DataTable l_Data = CommonUtils.ConvertCSVToDataTable(l_CSVData, columnNames);

                foreach(DataRow l_Row in l_Data.Rows)
                {
                    string l_TrackingNo = PublicFunctions.ConvertNullAsString(l_Row["TrackingNo"], string.Empty);
                    string l_DC = PublicFunctions.ConvertNullAsString(l_Row["DC"], string.Empty);
                    string l_StoreNo = PublicFunctions.ConvertNullAsString(l_Row["StoreNo"], string.Empty);
                    string l_Dept = PublicFunctions.ConvertNullAsString(l_Row["Dept"], string.Empty);

                    l_DC = l_DC.PadLeft(4, '0');
                    l_StoreNo = l_StoreNo.PadLeft(4, '0');
                    l_Dept = l_Dept.PadLeft(4, '0');
                    l_TrackingNo = l_TrackingNo.Replace(" ", "").Replace("(", "").Replace(")", "");

                    l_Row["TrackingNo"] = l_TrackingNo;
                    l_Row["DC"] = l_DC;
                    l_Row["StoreNo"] = l_StoreNo;
                    l_Row["Dept"] = l_Dept;
                }

                l_Data.AcceptChanges();

                l_Response = await OrderManager.ProcessASNStore(this._logger, OrderId, l_Data);

                l_Data.Dispose();
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }

            return l_Response;
        }

        [HttpPost]
        [Route("process810Store")]
        public async Task<OrderResponseModel> Process810ForStoreOrders(IFormFile file, int OrderId)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();

            OrderResponseModel l_Response = new OrderResponseModel();

            l_Response.Code = (int)ResponseCodes.Error;

            //try
            //{
            //    l_Response = await OrderManager.ProcessASN(this._logger, OrderId);
            //}
            //catch (Exception ex)
            //{
            //    l_Response.Code = (int)ResponseCodes.Exception;
            //    this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            //}

            return l_Response;
        }

        [HttpPost]
        [Route("markSendOrderStore")]
        public async Task<OrderStoreResponseModel> MarkStoreOrders(int OrderStoreId)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();

            OrderStoreResponseModel l_Response = new OrderStoreResponseModel();
            OrderStores l_OrderStore = new OrderStores();
            OrderObjectModel? l_Order = null;

            l_Response.Code = (int)ResponseCodes.Error;

            try
            {
                l_OrderStore.UseConnection(CommonUtils.ConnectionString);
                l_OrderStore.GetObject(OrderStoreId);
                
                l_Order = await OrderManager.GetOrder(l_OrderStore.OrderId);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Processing Mark for ASN for Store Order [{l_OrderStore.Id}].");

                ResponseModel l_SyncResponse = await OrderManager.MarkStoreOrdersForASN(l_Order.Customer, l_Order.Order, l_OrderStore.CustomerPO);

                if (l_SyncResponse.Code != (int)ResponseCodes.Success)
                {
                    this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {l_SyncResponse.Message}");

                    l_Response.Code = l_SyncResponse.Code;
                    l_Response.Message = l_SyncResponse.Message;

                    return l_Response;
                }

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Mark for ASN completed successfully.");

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Store Order marked for ASN successfully!";
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }

            return l_Response;
        }

        [HttpPost]
        [Route("createInvoiceOrderStore")]
        public async Task<OrderStoreResponseModel> CreateInvoiceStoreOrder(int OrderStoreId)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();

            OrderStoreResponseModel l_Response = new OrderStoreResponseModel();
            OrderStores l_OrderStore = new OrderStores();
            OrderObjectModel? l_Order = null;

            l_Response.Code = (int)ResponseCodes.Error;

            try
            {
                l_OrderStore.UseConnection(CommonUtils.ConnectionString);
                l_OrderStore.GetObject(OrderStoreId);

                l_Order = await OrderManager.GetOrder(l_OrderStore.OrderId);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Processing Create Invoice for Store Order [{l_OrderStore.Id}].");

                ResponseModel l_SyncResponse = await OrderManager.CreateStoreOrderInvoice(l_Order.Customer, l_Order.Order, l_OrderStore.CustomerPO);

                if (l_SyncResponse.Code != (int)ResponseCodes.Success)
                {
                    this._logger.LogError($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {l_SyncResponse.Message}");

                    l_Response.Code = l_SyncResponse.Code;
                    l_Response.Message = l_SyncResponse.Message;

                    return l_Response;
                }

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Create Invoice completed successfully.");

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Store Order Invoice created successfully!";
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                this._logger.LogCritical($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - {ex}");
            }

            return l_Response;
        }
    }
}
