using System.Data;
using System.Reflection.PortableExecutable;

namespace eSyncMate.Processor.Models
{
    internal enum ResponseCodes
    {
        Success = 200,
        Error = 400,
        Exception = 500,
        CustomerAlreadyExists = 401
    }
    public enum RouteTypesEnum
    {
        InventoryFeed = 1,
        GetOrders = 2,
        CreateOrder = 3,
        GetOrderStatus = 4,
        ASN = 5,
        CreateInvoice = 6
    }
    public enum ConnectorTypesEnum
    {
        SqlServer = 1,
        Rest = 2,
    }

    internal class CommonUtils
    {
        public static string ConnectionString { get; set; } = "Server=.;Database=EDIProcessor;UID=sa;PWD=angel;";

        public static RestSharp.Method GetRequestMethod(string p_Method)
        {
            RestSharp.Method l_Method = RestSharp.Method.Get;

            if(p_Method.Trim().ToLower() == "post")
            {
                l_Method = RestSharp.Method.Post;
            }
            else if (p_Method.Trim().ToLower() == "put")
            {
                l_Method = RestSharp.Method.Put;
            }

            return l_Method;
        }

        public static RestSharp.DataFormat GetRequestBodyFormat(string p_Format)
        {
            RestSharp.DataFormat l_Format = RestSharp.DataFormat.None;

            if (p_Format.Trim().ToLower() == "json")
            {
                l_Format = RestSharp.DataFormat.Json;
            }

            return l_Format;
        }

        public static DataTable ConvertCSVToDataTable(string fileData, string[] columnNames)
        {
            DataTable dataTable = new DataTable();
            int index = 0;

            foreach (string name in columnNames)
            {
                dataTable.Columns.Add(name);
            }

            foreach (string line in fileData.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            {
                if(index > 0)
                {
                    string[] rows = line.Split(',');
                    dataTable.Rows.Add(rows);
                }

                index ++;
            }

            return dataTable;
        }
    }
}
