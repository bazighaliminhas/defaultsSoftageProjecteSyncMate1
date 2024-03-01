using Microsoft.AspNetCore.Mvc;
using eSyncMate.DB.Entities;
using System.Reflection;
using eSyncMate.Processor.Models;
using System.Data;
using eSyncMate.DB;
using Hangfire;
using System.Text.Json;
using ExcelDataReader;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Numerics;
using Microsoft.AspNetCore.Authorization;

namespace eSyncMate.Processor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class CustomerProductCatalogController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;

        public CustomerProductCatalogController(ILogger<OrdersController> logger, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("getCustomerProductCatalog")]
        public async Task<GetCustomerProductCatalog> GetCustomerProductCatalog([FromQuery] CustomerProductCatalogSearchModel searchModel)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            GetCustomerProductCatalog l_Response = new GetCustomerProductCatalog();
            DataTable l_Data = new DataTable();
            string dateRange = string.Empty;
            string[] dateValues = new string[0];
            string startDate = string.Empty;
            string endDate = string.Empty;

            if (searchModel.SearchOption == "Created Date")
            {
                dateRange = searchModel.SearchValue;
                dateValues = dateRange.Split('/');
                startDate = dateValues[0].Trim() + " 00:00:00.000";
                endDate = dateValues[1].Trim() + " 23:59:59.999";
            }

            try
            {
                string l_Criteria = string.Empty;
                DB.Entities.CustomerProductCatalog l_CustomerProductCatalog = new DB.Entities.CustomerProductCatalog();

                l_Response.Code = (int)ResponseCodes.Error;

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Building search criteria.");

                l_CustomerProductCatalog.UseConnection(CommonUtils.ConnectionString);

                if (searchModel.SearchOption == "Created Date")
                {
                    l_Criteria += $" CreatedDate >= '{startDate}'";
                }

                if (searchModel.SearchOption == "Created Date")
                {
                    l_Criteria += $" AND CreatedDate <= '{endDate}'";
                }

                if (searchModel.SearchOption == "Id")
                {
                    l_Criteria = $" Id = {searchModel.SearchValue}";
                }
                else if (searchModel.SearchOption == "ERP CustomerID")
                {
                    l_Criteria = $" ERPCustomerID = '{searchModel.SearchValue}'";
                }
                else if (searchModel.SearchOption == "TCIN")
                {
                    l_Criteria = $" TCIN = '{searchModel.SearchValue}'";
                }
                else if (searchModel.SearchOption == "Partner SKU")
                {
                    l_Criteria = $" PartnerSKU = '{searchModel.SearchValue}'";
                }
                else if (searchModel.SearchOption == "ItemType")
                {
                    l_Criteria = $" ItemType = '{searchModel.SearchValue}'";
                }

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Search criteria ready ({l_Criteria}).");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring PartnerGroup search.");

                l_CustomerProductCatalog.GetViewList(l_Criteria, string.Empty, ref l_Data, "Id DESC");

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - CustomerProductCatalog searched {{{l_Data.Rows.Count}}}.");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Populating CustomerProductCatalog.");

                l_Response.CustomerProductCatalog = new List<CustomerProductCatalogDataModel>();
                foreach (DataRow l_Row in l_Data.Rows)
                {
                    CustomerProductCatalogDataModel l_CustomerProductCatalogRow = new CustomerProductCatalogDataModel();

                    DBEntity.PopulateObjectFromRow(l_CustomerProductCatalogRow, l_Data, l_Row);

                    l_Response.CustomerProductCatalog.Add(l_CustomerProductCatalogRow);
                }

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "CustomerProductCatalog fetched successfully!";

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - CustomerProductCatalog are ready.");
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
        [Route("createCustomerProductCatalog")]
        public async Task<GetCustomerProductCatalog> CreateCustomerProductCatalog([FromBody] SaveCustomerProductCatalogDataModel CustomerProductCatalogModel)
        {
            GetCustomerProductCatalog l_Response = new GetCustomerProductCatalog();
            Result l_Result = new Result();

            try
            {
                DB.Entities.CustomerProductCatalog l_CustomerProductCatalog = new DB.Entities.CustomerProductCatalog();
                l_CustomerProductCatalog.UseConnection(CommonUtils.ConnectionString);

                PublicFunctions.CopyTo(CustomerProductCatalogModel, l_CustomerProductCatalog);

                l_CustomerProductCatalog.CreatedBy = l_CustomerProductCatalog.CreatedBy;
                l_CustomerProductCatalog.CreatedDate = DateTime.Now;

                l_Result = l_CustomerProductCatalog.SaveNew();

                if (l_Result.IsSuccess)
                {
                    l_Response.Code = l_Result.Code;
                    l_Response.Message = l_Result.Description;
                    l_Response.Description = $"CustomerProductCatalog [ {CustomerProductCatalogModel.Id} ] has been created successfully!";
                }
                else
                {
                    l_Response.Code = (int)ResponseCodes.Error;
                    l_Response.Message = l_Result.Description;
                }

            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                l_Result.Description = ex.Message;
            }
            finally
            {

            }

            return l_Response;
        }

        [HttpPost]
        [Route("updateCustomerProductCatalog")]
        public async Task<GetCustomerProductCatalog> UpdateCustomerProductCatalog([FromBody] UpdateCustomerProductCatalogDataModel CustomerProductCatalogModel)
        {
            GetCustomerProductCatalog l_Response = new GetCustomerProductCatalog();
            Result l_Result = new Result();

            try
            {
                DB.Entities.CustomerProductCatalog l_CustomerProductCatalog = new DB.Entities.CustomerProductCatalog();
                l_CustomerProductCatalog.UseConnection(CommonUtils.ConnectionString);

                PublicFunctions.CopyTo(CustomerProductCatalogModel, l_CustomerProductCatalog);

                l_CustomerProductCatalog.ModifiedBy = l_CustomerProductCatalog.CreatedBy;
                l_CustomerProductCatalog.ModifiedDate = DateTime.Now;

                l_Result = l_CustomerProductCatalog.Modify();

                if (l_Result.IsSuccess)
                {
                    l_Response.Code = l_Result.Code;
                    l_Response.Message = l_Result.Description;
                    l_Response.Description = $"Map [ {CustomerProductCatalogModel.Id} ] has been updated successfully!";
                }
                else
                {
                    l_Response.Code = (int)ResponseCodes.Error;
                    l_Response.Message = l_Result.Description;
                }
            }
            catch (Exception ex)
            {
                l_Response.Code = (int)ResponseCodes.Exception;
                l_Result.Description = ex.Message;
            }
            finally
            {

            }

            return l_Response;
        }

        [HttpPost]
        [Route("processCustomerProductCatalogFile")]
        public async Task<GetCustomerProductCatalog> ProcessCustomerProductCatalogFile(IFormFile file, string ERPCustomerID)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            DataTable l_Data = new DataTable();
            GetCustomerProductCatalog l_Response = new GetCustomerProductCatalog();
            Result l_Result = new Result();
            CustomerProductCatalog l_CustomerProductCatalog = new CustomerProductCatalog();

            try
            {
                Customers l_Customer = new Customers();
                string l_Criteria = string.Empty;

                l_Customer.UseConnection(CommonUtils.ConnectionString);

                l_Criteria += $" ERPCustomerID = '{ERPCustomerID}'";

                l_Customer.GetList(l_Criteria, string.Empty, ref l_Data, "Id DESC");

                if (l_Data.Rows.Count == 0)
                {
                    l_Response.Code = 400;
                    l_Response.Message = $"Please provide a valid ERPCustomerID!";
                    l_Response.Description = $"Please provide a valid ERPCustomerID!";

                    return l_Response;
                }

                if (file == null || file.Length <= 0)
                {
                    l_Response.Code = 400;
                    l_Response.Message = $"  Please provide a product catalog data file!";
                    l_Response.Description = $"  Please provide a product catalog data file!";

                    return l_Response;
                }

                using (var stream = file.OpenReadStream())
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                    });

                    // Create a list to store tasks
                    var tasks = new List<Task>();

                    Parallel.ForEach(result.Tables.Cast<DataTable>(), table =>
                    {
                        // Use Task.Run to execute asynchronous bulk insert
                        tasks.Add(Task.Run(async () =>
                        {
                            await l_CustomerProductCatalog.BulkInsertSqlServer(table, CommonUtils.ConnectionString, ERPCustomerID, file.FileName, 1);
                        }));
                    });

                    // Wait for all tasks to complete
                    await Task.WhenAll(tasks);
                }

                l_Response.Code = 200;
                l_Response.Message = $" File has been processed successfully!";
                l_Response.Description = $" File has been processed successfully!";
            }
            catch (Exception ex)
            {
                l_Response.Code = 400;
                l_Response.Message = $"Invalid product catalog data file!";
                l_Response.Description = $"Invalid product catalog data file!";
            }

            return l_Response;
        }

        [HttpGet]
        [Route("getHistoryCustomerProductCatalog")]
        public async Task<GetCustomerProductCatalog_Log> GetHistoryCustomerProductCatalog(string ERPCustomerID)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            GetCustomerProductCatalog_Log l_Response = new GetCustomerProductCatalog_Log();
            DataTable l_Data = new DataTable();

            try
            {
                string l_Criteria = string.Empty;
                CustomerProductCatalog l_CustomerProductCatalog = new CustomerProductCatalog();

                l_Response.Code = (int)ResponseCodes.Error;

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Building search criteria.");

                l_CustomerProductCatalog.UseConnection(CommonUtils.ConnectionString);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Search criteria ready ({l_Criteria}).");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring History Customer Product Catalog.");

                l_CustomerProductCatalog.HistoryCustomerProductCatalog(ERPCustomerID, ref l_Data);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - History Customer Product Catalog searched {{{l_Data.Rows.Count}}}.");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Populating History Customer Product Catalog.");

                l_Response.CustomerProductCatalog_Log = new List<CustomerProductCatalog_LogDataModel>();
                foreach (DataRow l_Row in l_Data.Rows)
                {
                    CustomerProductCatalog_LogDataModel l_CustomerProductCatalog_LogRow = new CustomerProductCatalog_LogDataModel();

                    DBEntity.PopulateObjectFromRow(l_CustomerProductCatalog_LogRow, l_Data, l_Row);

                    l_Response.CustomerProductCatalog_Log.Add(l_CustomerProductCatalog_LogRow);
                }

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "History Customer Product Catalog fetched successfully!";

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - History Customer Product Catalog are ready.");
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
    }
}
