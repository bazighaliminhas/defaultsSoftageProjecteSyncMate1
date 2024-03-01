using Microsoft.AspNetCore.Mvc;
using eSyncMate.DB.Entities;
using System.Reflection;
using eSyncMate.Processor.Models;
using System.Data;
using eSyncMate.DB;
using Hangfire;
using System.Text.Json;
using eSyncMate.Processor.Managers;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;

namespace eSyncMate.Processor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class RoutesController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IConfiguration _config;

        public RoutesController(IConfiguration config, ILogger<OrdersController> logger, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        [Route("getRoutes")]
        public async Task<GetRoutesResponseModel> GetRoutes([FromQuery] RouteSearchModel searchModel)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            GetRoutesResponseModel l_Response = new GetRoutesResponseModel();
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
                DB.Entities.Routes l_Routes = new DB.Entities.Routes();

                l_Response.Code = (int)ResponseCodes.Error;

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Building search criteria.");

                l_Routes.UseConnection(CommonUtils.ConnectionString);

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
                else if (searchModel.SearchOption == "Source Party")
                {
                    l_Criteria = $" SourceParty = '{searchModel.SearchValue}'";
                }
                else if (searchModel.SearchOption == "Destination Party")
                {
                    l_Criteria = $" DestinationParty = '{searchModel.SearchValue}'";
                }
                else if (searchModel.SearchOption == "Source Connector")
                {
                    l_Criteria = $" SourceConnector = '{searchModel.SearchValue}'";
                }
                else if (searchModel.SearchOption == "Destination Connector")
                {
                    l_Criteria = $" DestinationConnector = '{searchModel.SearchValue}'";
                }
                else if (searchModel.SearchOption == "Party Group")
                {
                    l_Criteria = $" PartyGroup = '{searchModel.SearchValue}'";
                }

                else if (searchModel.SearchOption == "Route Type")
                {
                    l_Criteria = $" RouteType = '{searchModel.SearchValue}'";
                }

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Search criteria ready ({l_Criteria}).");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring PartnerGroup search.");

                l_Routes.GetViewList(l_Criteria, string.Empty, ref l_Data, "Id DESC");

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Routes searched {{{l_Data.Rows.Count}}}.");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Populating Routes.");

                l_Response.Routes = new List<RoutesDataModel>();
                foreach (DataRow l_Row in l_Data.Rows)
                {
                    RoutesDataModel l_RouteRow = new RoutesDataModel();

                    DBEntity.PopulateObjectFromRow(l_RouteRow, l_Data, l_Row);

                    l_Response.Routes.Add(l_RouteRow);
                }

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Routes fetched successfully!";

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Routes are ready.");
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
        [Route("createRoute")]
        public async Task<GetRoutesResponseModel> CreateRoute([FromBody] SaveRoutesDataModel routeModel)
        {
            GetRoutesResponseModel l_Response = new GetRoutesResponseModel();
            Result l_Result = new Result();
            string l_JobID = string.Empty;

            try
            {
                DB.Entities.Routes l_Routes = new DB.Entities.Routes();
                l_Routes.UseConnection(CommonUtils.ConnectionString);

                PublicFunctions.CopyTo(routeModel, l_Routes);

                l_Routes.CreatedBy = l_Routes.CreatedBy;
                l_Routes.CreatedDate = DateTime.Now;

                if (l_Routes.Status == "Active")
                {
                    l_JobID = this.SetupRouteJob(l_Routes);
                    l_Routes.JobID = l_JobID;
                }

                l_Result = l_Routes.SaveNew();

                if (l_Result.IsSuccess)
                {
                    l_Response.Code = l_Result.Code;
                    l_Response.Message = l_Result.Description;
                    l_Response.Description = $"Routes [ {routeModel.Id} ] has been created successfully!";
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
        [Route("updateRoute")]
        public async Task<GetRoutesResponseModel> UpdateRoute([FromBody] UpdateRoutesDataModel RoutesModel)
        {
            GetRoutesResponseModel l_Response = new GetRoutesResponseModel();
            Result l_Result = new Result();
            string l_JobID = string.Empty;

            try
            {
                DB.Entities.Routes l_Routes = new DB.Entities.Routes();
                DB.Entities.Routes l_OldRoute = new DB.Entities.Routes();

                l_Routes.UseConnection(CommonUtils.ConnectionString);
                l_OldRoute.UseConnection(CommonUtils.ConnectionString);

                PublicFunctions.CopyTo(RoutesModel, l_Routes);

                l_Routes.ModifiedBy = l_Routes.CreatedBy;
                l_Routes.ModifiedDate = DateTime.Now;

                l_OldRoute.GetObject(l_Routes.Id);

                if (l_OldRoute.Status == "Active")
                {
                    this.RemoveRouteJob(l_OldRoute);
                    l_Routes.JobID = null;
                }

                if (l_Routes.Status == "Active")
                {
                    l_JobID = this.SetupRouteJob(l_Routes);
                    l_Routes.JobID = l_JobID;
                }

                l_Result = l_Routes.Modify();

                if (l_Result.IsSuccess)
                {
                    l_Response.Code = l_Result.Code;
                    l_Response.Message = l_Result.Description;
                    l_Response.Description = $"Map [ {RoutesModel.Id} ] has been updated successfully!";
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

        [HttpGet]
        [Route("getCustomers")]
        public async Task<GetCustomersResponseModel> GetCustomers()
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            GetCustomersResponseModel l_Response = new GetCustomersResponseModel();
            DataTable l_Data = new DataTable();

            try
            {
                string l_Criteria = string.Empty;
                Customers l_Customer = new Customers();

                l_Response.Code = (int)ResponseCodes.Error;

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Building search criteria.");

                l_Customer.UseConnection(CommonUtils.ConnectionString);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Search criteria ready ({l_Criteria}).");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring Partner Groups search.");

                l_Customer.GetList(l_Criteria, string.Empty, ref l_Data, "Id DESC");

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Routes searched {{{l_Data.Rows.Count}}}.");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Populating Partner Groups.");

                l_Response.Customers = new List<CustomerDataModel>();
                foreach (DataRow l_Row in l_Data.Rows)
                {
                    CustomerDataModel l_CustomerRow = new CustomerDataModel();

                    DBEntity.PopulateObjectFromRow(l_CustomerRow, l_Data, l_Row);

                    l_Response.Customers.Add(l_CustomerRow);
                }

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Partner Groups fetched successfully!";

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Partner Groups are ready.");
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
        [Route("getConnectors")]
        public async Task<GetConnectorsResponseModel> GetConnectors()
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            GetConnectorsResponseModel l_Response = new GetConnectorsResponseModel();
            DataTable l_Data = new DataTable();

            try
            {
                string l_Criteria = string.Empty;
                Connectors l_Connectors = new Connectors();

                l_Response.Code = (int)ResponseCodes.Error;

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Building search criteria.");

                l_Connectors.UseConnection(CommonUtils.ConnectionString);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Search criteria ready ({l_Criteria}).");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring Routes search.");

                l_Connectors.GetList(l_Criteria, string.Empty, ref l_Data, "Id DESC");

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Routes searched {{{l_Data.Rows.Count}}}.");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Populating Partner Groups.");

                l_Response.Connectors = new List<ConnectorsDataModel>();
                foreach (DataRow l_Row in l_Data.Rows)
                {
                    ConnectorsDataModel l_ConnectorRow = new ConnectorsDataModel();

                    DBEntity.PopulateObjectFromRow(l_ConnectorRow, l_Data, l_Row);

                    l_Response.Connectors.Add(l_ConnectorRow);
                }

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Connectors fetched successfully!";

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Connectors are ready.");
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
        [Route("getMaps")]
        public async Task<GetMapsResponseModel> GetMaps()
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            GetMapsResponseModel l_Response = new GetMapsResponseModel();
            DataTable l_Data = new DataTable();

            try
            {
                string l_Criteria = string.Empty;
                DB.Entities.Maps l_Map = new DB.Entities.Maps();

                l_Response.Code = (int)ResponseCodes.Error;

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Building search criteria.");

                l_Map.UseConnection(CommonUtils.ConnectionString);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Search criteria ready ({l_Criteria}).");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring Routes search.");

                l_Map.GetList(l_Criteria, string.Empty, ref l_Data, "Id DESC");

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Maps searched {{{l_Data.Rows.Count}}}.");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Populating Maps.");

                l_Response.Maps = new List<MapDataModel>();
                foreach (DataRow l_Row in l_Data.Rows)
                {
                    MapDataModel l_MapsRow = new MapDataModel();

                    DBEntity.PopulateObjectFromRow(l_MapsRow, l_Data, l_Row);

                    l_Response.Maps.Add(l_MapsRow);
                }

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Maps fetched successfully!";

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Maps are ready.");
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
        [Route("getRouteTypes")]
        public async Task<GetRouteTypeResponseModel> GetRouteTypes()
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            GetRouteTypeResponseModel l_Response = new GetRouteTypeResponseModel();
            DataTable l_Data = new DataTable();

            try
            {
                string l_Criteria = string.Empty;
                RouteTypes l_RouteType = new RouteTypes();

                l_Response.Code = (int)ResponseCodes.Error;

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Building search criteria.");

                l_RouteType.UseConnection(CommonUtils.ConnectionString);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Search criteria ready ({l_Criteria}).");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring Route Type search.");

                l_RouteType.GetList(l_Criteria, string.Empty, ref l_Data, "Id DESC");

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Route Type searched {{{l_Data.Rows.Count}}}.");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Populating Route Type.");

                l_Response.RouteType = new List<RouteTypeDataModel>();
                foreach (DataRow l_Row in l_Data.Rows)
                {
                    RouteTypeDataModel l_RouteTypeRow = new RouteTypeDataModel();

                    DBEntity.PopulateObjectFromRow(l_RouteTypeRow, l_Data, l_Row);

                    l_Response.RouteType.Add(l_RouteTypeRow);
                }

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Route Type fetched successfully!";

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Route Type are ready.");
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
        [Route("getPartnerGroup")]
        public async Task<GetPartnerGroupsResponseModel> GetPartnerGroup()
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            GetPartnerGroupsResponseModel l_Response = new GetPartnerGroupsResponseModel();
            DataTable l_Data = new DataTable();

            try
            {
                string l_Criteria = string.Empty;
                PartnerGroups l_PartnerGroup = new PartnerGroups();

                l_Response.Code = (int)ResponseCodes.Error;

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Building search criteria.");

                l_PartnerGroup.UseConnection(CommonUtils.ConnectionString);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Search criteria ready ({l_Criteria}).");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring Partner Group search.");

                l_PartnerGroup.GetList(l_Criteria, string.Empty, ref l_Data, "Id DESC");

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Partner Group searched {{{l_Data.Rows.Count}}}.");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Populating Partner Group.");

                l_Response.PartnerGroup = new List<PartnerGroupDataModel>();
                foreach (DataRow l_Row in l_Data.Rows)
                {
                    PartnerGroupDataModel l_PartnerGroupRow = new PartnerGroupDataModel();

                    DBEntity.PopulateObjectFromRow(l_PartnerGroupRow, l_Data, l_Row);

                    l_Response.PartnerGroup.Add(l_PartnerGroupRow);
                }

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Partner Group Type fetched successfully!";

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Partner Group are ready.");
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

        private string SetupRouteJob(Routes route)
        {
            RouteEngine l_Engine = new RouteEngine(this._config, this._logger);

            return BackgroundJob.Schedule(() => l_Engine.Schedule(route.Id), route.StartDate - DateTime.Now);
        }

        private void RemoveRouteJob(Routes route)
        {
            RouteEngine l_Engine = new RouteEngine(this._config, this._logger);

            l_Engine.RemoveRouteJob(route);
        }

        [HttpGet]
        [Route("getRouteLog")]
        public async Task<GetRouteLogResponseModel> GetRouteLog(int RouteID)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            GetRouteLogResponseModel l_Response = new GetRouteLogResponseModel();
            DataTable l_Data = new DataTable();

            try
            {
                string l_Criteria = string.Empty;
                RouteLog l_RouteLog = new RouteLog();

                l_Response.Code = (int)ResponseCodes.Error;

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Building search criteria.");

                l_RouteLog.UseConnection(CommonUtils.ConnectionString);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Search criteria ready ({l_Criteria}).");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring History Customer Product Catalog.");

                l_RouteLog.GetRouteLog(RouteID, ref l_Data);

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - History Customer Product Catalog searched {{{l_Data.Rows.Count}}}.");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Populating History Customer Product Catalog.");

                l_Response.RouteLog = new List<RouteLogDataModel>();
                foreach (DataRow l_Row in l_Data.Rows)
                {
                    RouteLogDataModel l_RouteLogDataRow = new RouteLogDataModel();

                    DBEntity.PopulateObjectFromRow(l_RouteLogDataRow, l_Data, l_Row);

                    l_Response.RouteLog.Add(l_RouteLogDataRow);
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
