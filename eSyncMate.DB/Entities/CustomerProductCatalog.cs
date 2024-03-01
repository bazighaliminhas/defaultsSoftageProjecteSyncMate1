using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace eSyncMate.DB.Entities
{
    public class CustomerProductCatalog : DBEntity, IDBEntity, IDisposable, IEqualityComparer
    {
        public int Id { get; set; }
        public string ERPCustomerID { get; set; }
        public string TCIN { get; set; }
        public string PartnerSKU { get; set; }
        public string ProductTitle { get; set; }
        public string ItemType { get; set; }
        public string ItemTypeID { get; set; }
        public string Relationship { get; set; }
        public string PublishStatus { get; set; }
        public string DataUpdatesStatus { get; set; }
        public decimal Price { get; set; }
        public decimal OfferPrice { get; set; }
        public decimal MAPPrice { get; set; }
        public decimal OfferDiscount { get; set; }
        public DateTime PriceLastUpdated { get; set; }
        public string DistributionCenterName { get; set; }
        public string DistributionCenterID { get; set; }
        public int Inventory { get; set; }
        public DateTime InventoryLastUpdated { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        private static string TableName { get; set; }
        private static string ViewName { get; set; }
        private static string PrimaryKeyName { get; set; }
        private static string InsertQueryStart { get; set; }
        private static string EndingPropertyName { get; set; }
        public static List<PropertyInfo> DBProperties { get; set; }
        public Customers SourcePartyObject { get; set; }
        public Customers DestinationPartyObject { get; set; }
        public Connectors SourceConnectorObject { get; set; }
        public Connectors DestinationConnectorObject { get; set; }
        public Maps MapObject { get; set; }


        /// <summary>
        /// TODO: Update summary.
        /// </summary>
        public CustomerProductCatalog() : base()
        {
            SetupDBEntity();
        }

        /// <summary>
        /// TODO: Update summary.
        /// </summary>
        public CustomerProductCatalog(DBConnector p_Connection) : base(p_Connection)
        {
            SetupDBEntity();
        }

        /// <summary>
        /// TODO: Update summary.
        /// </summary>
        public CustomerProductCatalog(string p_ConnectionString) : base(p_ConnectionString)
        {
            SetupDBEntity();
        }

        /// <summary>
        /// TODO: Update summary.
        /// </summary>
        private void SetupDBEntity()
        {
            string l_Query = string.Empty;

            if (string.IsNullOrEmpty(CustomerProductCatalog.TableName))
            {
                CustomerProductCatalog.TableName = "CustomerProductCatalog";
            }

            if (string.IsNullOrEmpty(CustomerProductCatalog.ViewName))
            {
                CustomerProductCatalog.ViewName = "VW_CustomerProductCatalog";
            }

            if (string.IsNullOrEmpty(CustomerProductCatalog.PrimaryKeyName))
            {
                CustomerProductCatalog.PrimaryKeyName = "Id";
            }

            if (string.IsNullOrEmpty(CustomerProductCatalog.EndingPropertyName))
            {
                CustomerProductCatalog.EndingPropertyName = "CreatedBy";
            }

            if (CustomerProductCatalog.DBProperties == null)
            {
                CustomerProductCatalog.DBProperties = new List<PropertyInfo>(this.GetType().GetProperties());
            }

            if (string.IsNullOrEmpty(CustomerProductCatalog.InsertQueryStart))
            {
                CustomerProductCatalog.InsertQueryStart = PrepareQueries(this, CustomerProductCatalog.TableName, CustomerProductCatalog.EndingPropertyName, ref l_Query, CustomerProductCatalog.DBProperties);
            }
        }

        public void UseConnection(string p_ConnectionString, DBConnector p_Connection = null)
        {
            if (string.IsNullOrEmpty(p_ConnectionString))
            {
                Connection = p_Connection;
            }
            else
            {
                Connection = new DBConnector(p_ConnectionString);
            }
        }

        /// <summary>
        /// TODO: Update summary.
        /// </summary>
        public bool GetList(string p_Criteria, string p_Fields, ref DataTable p_Data, string p_OrderBy = "")
        {
            string l_Query = string.Empty;

            if (string.IsNullOrEmpty(p_Fields))
            {
                l_Query = "SELECT * FROM [" + CustomerProductCatalog.TableName + "]";
            }
            else
            {
                l_Query = "SELECT " + p_Fields + " FROM [" + CustomerProductCatalog.TableName + "]";
            }

            if (!string.IsNullOrEmpty(p_Criteria))
            {
                l_Query += " WHERE " + p_Criteria;
            }

            if (!string.IsNullOrEmpty(p_OrderBy))
            {
                l_Query += " ORDER BY " + p_OrderBy;
            }

            return Connection.GetData(l_Query, ref p_Data);
        }

        public bool GetViewList(string p_Criteria, string p_Fields, ref DataTable p_Data, string p_OrderBy = "")
        {
            string l_Query = string.Empty;

            if (string.IsNullOrEmpty(p_Fields))
            {
                l_Query = "SELECT * FROM [" + CustomerProductCatalog.ViewName + "]";
            }
            else
            {
                l_Query = "SELECT " + p_Fields + " FROM [" + CustomerProductCatalog.ViewName + "]";
            }

            if (!string.IsNullOrEmpty(p_Criteria))
            {
                l_Query += " WHERE " + p_Criteria;
            }

            if (!string.IsNullOrEmpty(p_OrderBy))
            {
                l_Query += " ORDER BY " + p_OrderBy;
            }

            return Connection.GetData(l_Query, ref p_Data);
        }

        /// <summary>
        /// TODO: Update summary.
        /// </summary>
        public Result GetObject(int p_PrimaryKey)
        {
            SetProperty(CustomerProductCatalog.PrimaryKeyName, p_PrimaryKey);
            return GetObjectFromQuery(PrepareGetObjectQuery(this, CustomerProductCatalog.ViewName, CustomerProductCatalog.PrimaryKeyName));
        }

        public int GetMax()
        {
            DataTable l_Data = new DataTable();
            int l_MaxNo = 1;
            Common l_Common = new Common();

            l_Common.UseConnection(string.Empty, Connection);
            if (!l_Common.GetList("SELECT MAX(CONVERT(INT, ISNULL(" + CustomerProductCatalog.PrimaryKeyName + ", '0'))) FROM " + CustomerProductCatalog.TableName, ref l_Data))
            {
                return l_MaxNo;
            }

            l_MaxNo = PublicFunctions.ConvertNullAsInteger(l_Data.Rows[0][0], 0) + 1;

            l_Data.Dispose();

            return l_MaxNo;
        }

        public Result GetObjectFromQuery(string p_Query, bool isOnlyObject = false)
        {
            string l_Query = string.Empty;
            string l_Param = string.Empty;
            string l_Criteria = string.Empty;
            DataTable l_Data = new DataTable();

            if (!Connection.GetData(p_Query, ref l_Data))
            {
                return Result.GetNoRecordResult();
            }

            PopulateObject(this, l_Data, DBProperties, string.Empty);

            l_Data.Dispose();
            if (isOnlyObject)
            {
                return Result.GetSuccessResult();
            }

            return Result.GetSuccessResult();
        }

        public Result GetObject()
        {
            return GetObjectFromQuery(PrepareGetObjectQuery(this, CustomerProductCatalog.ViewName, CustomerProductCatalog.PrimaryKeyName));
        }

        /// <summary>
        /// TODO: Update summary.
        /// </summary>
        public Result GetObjectOnly(int p_PrimaryKey)
        {
            SetProperty(CustomerProductCatalog.PrimaryKeyName, p_PrimaryKey);
            return GetObjectFromQuery(PrepareGetObjectQuery(this, CustomerProductCatalog.ViewName, CustomerProductCatalog.PrimaryKeyName), true);
        }
        public Result GetObjectOnly()
        {
            return GetObjectFromQuery(PrepareGetObjectQuery(this, CustomerProductCatalog.ViewName, CustomerProductCatalog.PrimaryKeyName), true);
        }

        public Result GetObject(string propertyName, object propertyValue)
        {
            var l_Property = this.GetType().GetProperties().Where(p => p.Name == propertyName);

            l_Property.FirstOrDefault<PropertyInfo>()?.SetValue(this, propertyValue);

            return GetObjectFromQuery(PrepareGetObjectQuery(this, CustomerProductCatalog.ViewName, propertyName));
        }

        public Result SaveNew()
        {
            Result l_Result = Result.GetFailureResult();
            bool l_Trans = false;
            bool l_Process = false;
            string l_Query = string.Empty;

            try
            {
                l_Trans = this.Connection.BeginTransaction();

                this.Id = this.GetMax();

                l_Query = this.PrepareInsertQuery(this, CustomerProductCatalog.InsertQueryStart, CustomerProductCatalog.EndingPropertyName, CustomerProductCatalog.DBProperties);

                l_Process = this.Connection.Execute(l_Query);

                if (l_Trans)
                {
                    if (l_Process)
                    {
                        this.Connection.CommitTransaction();
                        l_Result = Result.GetSuccessResult();
                    }
                    else
                    {
                        this.Connection.RollbackTransaction();
                    }
                }
            }
            catch (Exception)
            {
                if (l_Trans)
                {
                    this.Connection.RollbackTransaction();
                }

                throw;
            }
            finally
            {
            }

            return l_Result;
        }

        public Result Modify()
        {
            Result l_Result = Result.GetFailureResult();
            bool l_Trans = false;
            bool l_Process = false;
            string l_Query = string.Empty;

            try
            {
                l_Trans = this.Connection.BeginTransaction();

                l_Query = this.PrepareUpdateQuery(this, CustomerProductCatalog.TableName, CustomerProductCatalog.PrimaryKeyName, CustomerProductCatalog.EndingPropertyName, CustomerProductCatalog.DBProperties);

                l_Process = this.Connection.Execute(l_Query);

                if (l_Trans)
                {
                    if (l_Process)
                    {
                        this.Connection.CommitTransaction();
                        l_Result = Result.GetSuccessResult();
                    }
                    else
                    {
                        this.Connection.RollbackTransaction();
                    }
                }
            }
            catch (Exception)
            {
                if (l_Trans)
                {
                    this.Connection.RollbackTransaction();
                }

                throw;
            }
            finally
            {
            }

            return l_Result;
        }

        public Result Delete()
        {
            Result l_Result = Result.GetFailureResult();
            bool l_Trans = false;
            bool l_Process = false;
            string l_Query = string.Empty;

            try
            {
                l_Trans = this.Connection.BeginTransaction();

                l_Query = this.PrepareDeleteQuery(this, CustomerProductCatalog.TableName, CustomerProductCatalog.PrimaryKeyName);

                l_Process = this.Connection.Execute(l_Query);

                if (l_Trans)
                {
                    if (l_Process)
                    {
                        this.Connection.CommitTransaction();
                        l_Result = Result.GetSuccessResult();
                    }
                    else
                    {
                        this.Connection.RollbackTransaction();
                    }
                }
            }
            catch (Exception)
            {
                if (l_Trans)
                {
                    this.Connection.RollbackTransaction();
                }

                throw;
            }
            finally
            {
            }

            return l_Result;
        }

        public Result SaveData(int userNo, string Param1, string Param2)
        {
            Result l_Result = Result.GetFailureResult();
            bool l_Trans = false;
            string l_Query = string.Empty;
            string l_Param = string.Empty;

            try
            {
                l_Trans = this.Connection.BeginTransaction();

                l_Query = "EXEC sp_SaveData (";

                PublicFunctions.FieldToParam(userNo, ref l_Param, Declarations.FieldTypes.NullableNumber);
                l_Query += l_Param;
                PublicFunctions.FieldToParam(Param1, ref l_Param, Declarations.FieldTypes.String);
                l_Query += ", " + l_Param;
                PublicFunctions.FieldToParam(Param2, ref l_Param, Declarations.FieldTypes.String);
                l_Query += ", " + l_Param + ")";

                this.Connection.ExecuteScalar(l_Query);

                if (l_Trans)
                {
                    this.Connection.CommitTransaction();
                    l_Result = Result.GetSuccessResult();
                }
            }
            catch (Exception ex)
            {
                if (l_Trans)
                {
                    this.Connection.RollbackTransaction();
                    l_Result.Description = ex.Message;
                }

                throw;
            }
            finally
            {
            }

            return l_Result;
        }

        public Result SaveLog(int userNo, string Param1, string Param2)
        {
            Result l_Result = Result.GetFailureResult();
            bool l_Trans = false;
            string l_Query = string.Empty;
            string l_Param = string.Empty;

            try
            {
                l_Trans = this.Connection.BeginTransaction();

                l_Query = "EXEC sp_SaveLog (";

                PublicFunctions.FieldToParam(userNo, ref l_Param, Declarations.FieldTypes.NullableNumber);
                l_Query += l_Param;
                PublicFunctions.FieldToParam(Param1, ref l_Param, Declarations.FieldTypes.String);
                l_Query += ", " + l_Param;
                PublicFunctions.FieldToParam(Param2, ref l_Param, Declarations.FieldTypes.String);
                l_Query += ", " + l_Param + ")";

                this.Connection.ExecuteScalar(l_Query);

                if (l_Trans)
                {
                    this.Connection.CommitTransaction();
                    l_Result = Result.GetSuccessResult();
                }
            }
            catch (Exception ex)
            {
                if (l_Trans)
                {
                    this.Connection.RollbackTransaction();
                    l_Result.Description = ex.Message;
                }

                throw;
            }
            finally
            {
            }

            return l_Result;
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }

            disposedValue = true;
        }

        // TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        // Protected Overrides Sub Finalize()
        // ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        // Dispose(False)
        // MyBase.Finalize()
        // End Sub

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region IEqualityComparer Support
        public new bool Equals(object x, object y)
        {
            return ((CustomerProductCatalog)x).Id == ((CustomerProductCatalog)y).Id;
        }

        public new int GetHashCode(object obj)
        {
            return this.Id;
        }
        #endregion

        public async Task BulkInsertSqlServer(DataTable dataTable, string connectionString, string ERPCustomerID, string FileName, Int16 CreatedBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SP_" + CustomerProductCatalog.TableName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@p_ERPCustomerID", SqlDbType.NVarChar).Value = ERPCustomerID;
                        command.Parameters.Add("@p_FileName", SqlDbType.NVarChar).Value = FileName;
                        command.Parameters.Add("@p_CreatedBy", SqlDbType.Int).Value = CreatedBy;

                        SqlParameter parameterDataTable = new SqlParameter("@p_DataTable", SqlDbType.Structured)
                        {
                            Value = dataTable,
                            TypeName = CustomerProductCatalog.TableName + "Type"
                        };
                        command.Parameters.Add(parameterDataTable);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid file data: " + ex.Message);
                throw;
            }
        }

        public  bool HistoryCustomerProductCatalog(string p_ERPCustomerID, ref DataTable p_Data)
        {
            string l_SQL = string.Empty;
            string l_Param = string.Empty;
            string l_criteria = string.Empty;
            var l_dt = new DataTable();
            
            l_SQL = "SELECT ERPCustomerID,FileName,CreatedDate,CreatedBy FROM CustomerProductCatalog_Log WITH (NOLOCK)";
            PublicFunctions.FieldToParam(p_ERPCustomerID, ref l_Param, Declarations.FieldTypes.String);
            l_SQL += " WHERE ERPCustomerID = " + l_Param;


            return Connection.GetData(l_SQL, ref p_Data);
        }

    }
}
