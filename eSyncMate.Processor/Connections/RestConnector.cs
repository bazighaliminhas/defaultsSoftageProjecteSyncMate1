
using eSyncMate.Processor.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth;
using System;


namespace eSyncMate.Processor.Connections
{
    public class RestConnector
    {
        public static async Task<RestResponse> Execute(ConnectorDataModel connector, string body)
        {
            try
            {
                RestClient client = new RestClient();
                RestRequest request = new RestRequest();
                RestResponse response = new RestResponse();

                if (!string.IsNullOrEmpty(connector.ConsumerKey) && !string.IsNullOrEmpty(connector.ConsumerSecret))
                {
                    RestClientOptions options = new RestClientOptions(connector.BaseUrl)
                    {
                        MaxTimeout = -1,
                    };

                    if (connector.AuthType == "Auth1")
                    {
                        OAuth1Authenticator l_Auth1 = OAuth1Authenticator.ForAccessToken(
                                consumerKey: connector.ConsumerKey,
                                consumerSecret: connector.ConsumerSecret,
                                token: connector.Token,
                                tokenSecret: connector.TokenSecret,
                                OAuthSignatureMethod.HmacSha256);
                        l_Auth1.Realm = connector.Realm;

                        options.Authenticator = l_Auth1;
                    }

                    client = new RestClient(options);
                }

                request = new RestRequest(connector.Url, CommonUtils.GetRequestMethod(connector.Method));

                if (connector.Parmeters != null)
                {
                    foreach (Models.Parameter l_Parameter in connector.Parmeters)
                    {
                        request.AddQueryParameter(l_Parameter.Name, l_Parameter.Value);
                    }
                }

                if (connector.Headers != null)
                {
                    foreach (Header l_Header in connector.Headers)
                    {
                        request.AddHeader(l_Header.Name, l_Header.Value);
                    }
                }

                if (!string.IsNullOrEmpty(body))
                {
                    request.AddStringBody(body, CommonUtils.GetRequestBodyFormat(connector.BodyFormat));
                }

                response = await client.ExecuteAsync(request);

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}