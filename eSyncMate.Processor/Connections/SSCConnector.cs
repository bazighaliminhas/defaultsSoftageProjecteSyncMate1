using eSyncMate.DB.Entities;
using eSyncMate.Processor.Models;
using Newtonsoft.Json;
using RestSharp.Authenticators.OAuth;
using RestSharp.Authenticators;
using RestSharp;
using eSyncMate.DB;
using Nancy;
using Microsoft.IdentityModel.Tokens;
using Intercom.Data;

namespace eSyncMate.Processor.Connections
{
    public class SSCConnector : RestConnector,IConnector
    {
        public static string Token = string.Empty;

        public static string BaseURL = "https://api.safavieh.com/SPARS.API_UAT/api/service/Get_Token?key=07453D34-6A64-431A-939A-7AC666338427&Company=eMS";

        public async Task GetApiToken()
        {
            RestClient client = new RestClient();
            RestRequest request = new RestRequest();
            RestResponse response = new RestResponse();
            try
            {
                request = new RestRequest(SSCConnector.BaseURL, eSyncMate.Processor.Models.CommonUtils.GetRequestMethod("GET"));

                response = await client.ExecuteAsync(request);

                var tokenInfoDefinition = new
                {
                    Code = "",
                    Description = "",
                    Token = ""
                };

                var tokenInfo = JsonConvert.DeserializeAnonymousType(response.Content, tokenInfoDefinition);
                SSCConnector.Token = tokenInfo.Token;

            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public static async Task<bool> CreateOrder(int p_OrderId)
        {
            Orders l_Order = new Orders();
            Customers l_Customer = new Customers();
            ConnectorDataModel? l_Data = null;

            l_Order.UseConnection(Models.CommonUtils.ConnectionString);
            if (!l_Order.GetObject(p_OrderId).IsSuccess)
            {
                return false;
            }

            l_Customer.UseConnection("", l_Order.Connection);
            l_Customer.GetObject(l_Order.CustomerId);

            var l_OrderData = l_Order.Files.Where(f => f.Type == "850-JSON");

            if (l_OrderData == null)
            {
                return false;
            }

            string json = l_OrderData.FirstOrDefault<OrderData>().Data;
            var l_Connector = l_Customer.Connectors.Where(c => c.ConnectorName == "Create Order" && c.ConnectorType == "Orders" && c.ConnectorParty == "NetSuite");

            if (l_Connector != null)
            {
                l_Data = JsonConvert.DeserializeObject<ConnectorDataModel>(l_Connector.FirstOrDefault<CustomerConnectors>()?.ConnectorData);
            }

            if (l_Data == null)
            {
                return false;
            }

            RestResponse response = await Execute(l_Data, json);

            return true;
        }

    }
}