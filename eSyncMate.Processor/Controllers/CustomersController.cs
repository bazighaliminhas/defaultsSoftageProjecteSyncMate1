using Microsoft.AspNetCore.Mvc;
using eSyncMate.DB.Entities;
using System.Reflection;
using eSyncMate.Processor.Models;
using System.Data;
using eSyncMate.DB;
using Hangfire;
using Microsoft.AspNetCore.Authorization;

namespace eSyncMate.Processor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;

        public CustomersController(ILogger<OrdersController> logger, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("getCustomers")]
        public async Task<GetCustomersResponseModel> GetCustomers([FromQuery] CustomerSearchModel searchModel)
        {
            MethodBase l_Me = MethodBase.GetCurrentMethod();
            GetCustomersResponseModel l_Response = new GetCustomersResponseModel();
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
                Customers l_Customer = new Customers();

                l_Response.Code = (int)ResponseCodes.Error;

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Building search criteria.");

                l_Customer.UseConnection(CommonUtils.ConnectionString);

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
                else if (searchModel.SearchOption == "Customer Name")
                {
                    l_Criteria = $" Name = '{searchModel.SearchValue}'";
                }
                else if (searchModel.SearchOption == "ERP Customer ID")
                {
                    l_Criteria = $" ERPCustomerID = '{searchModel.SearchValue}'";
                }
                else if (searchModel.SearchOption == "ISA Customer ID")
                {
                    l_Criteria = $" ISACustomerID = '{searchModel.SearchValue}'";
                }
                else if (searchModel.SearchOption == "ISA 810 Receiver ID")
                {
                    l_Criteria = $" ISA810ReceiverId = '{searchModel.SearchValue}'";
                }
                else if (searchModel.SearchOption == "Market Place")
                {
                    l_Criteria = $" Marketplace = '{searchModel.SearchValue}'";
                }

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Search criteria ready ({l_Criteria}).");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Staring Customer search.");

                l_Customer.GetList(l_Criteria, string.Empty, ref l_Data, "Id DESC");

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Customers searched {{{l_Data.Rows.Count}}}.");
                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Populating Customers.");

                l_Response.Customers = new List<CustomerDataModel>();
                foreach (DataRow l_Row in l_Data.Rows)
                {
                    CustomerDataModel l_CustomerRow = new CustomerDataModel();

                    DBEntity.PopulateObjectFromRow(l_CustomerRow, l_Data, l_Row);

                    l_Response.Customers.Add(l_CustomerRow);
                }

                l_Response.Code = (int)ResponseCodes.Success;
                l_Response.Message = "Customers fetched successfully!";

                this._logger.LogDebug($"[{l_Me.ReflectedType.Name}.{l_Me.Name}] - Customers are ready.");
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
        [Route("createCustomer")]
        public async Task<CustomersResponseModel> CreateCustomer([FromBody] SaveCustomerDataModel customerModel)
        {
            CustomersResponseModel l_Response = new CustomersResponseModel();
            Result l_Result = new Result();

            try
            {
                Customers l_Customer = new Customers();
                l_Customer.UseConnection(CommonUtils.ConnectionString);

                if (l_Customer.GetObject("Name", customerModel.Name).IsSuccess)
                {
                    l_Response.Code = (int)ResponseCodes.CustomerAlreadyExists;
                    l_Response.Description = $"This customer [ {customerModel.Name} ] is already Exists!";

                    return l_Response;
                }

                PublicFunctions.CopyTo(customerModel, l_Customer);

                l_Customer.CreatedBy = l_Customer.CreatedBy;
                l_Customer.CreatedDate = DateTime.Now;

                l_Result = l_Customer.SaveNew();

                if (l_Result.IsSuccess)
                {
                    l_Response.Code = l_Result.Code;
                    l_Response.Message = l_Result.Description;
                    l_Response.Description = $"Customer [ {customerModel.Name} ] has been created successfully!";
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
        [Route("updateCustomer")]
        public async Task<CustomersResponseModel> UpdateCustomer([FromBody] EditCustomerDataModel customerModel)
        {
            CustomersResponseModel l_Response = new CustomersResponseModel();
            Result l_Result = new Result();

            try
            {
                Customers l_Customer = new Customers();
                l_Customer.UseConnection(CommonUtils.ConnectionString);

                PublicFunctions.CopyTo(customerModel, l_Customer);

                l_Customer.ModifiedBy = l_Customer.CreatedBy;
                l_Customer.ModifiedDate = DateTime.Now;

                l_Result = l_Customer.Modify();

                if (l_Result.IsSuccess)
                {
                    l_Response.Code = l_Result.Code;
                    l_Response.Message = l_Result.Description;
                    l_Response.Description = $"Customer [ {customerModel.Name} ] has been updated successfully!";
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

    }
}
