using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace OrionDAL.DAL
{
    public class DataAccess
    {
        public static IConnectionHandle GetConnection(string connectionString, string dbType)
        {
            DbProviderFactory factory;
            DbConnection connection;
            ConnectionHandle handle;

            try
            {
                factory = GetFactory(dbType);
            }
            catch (Exception exception)
            {
                throw new Exception("DB: " + dbType, exception);
            }                
            
            connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            
            handle = new ConnectionHandle(connection, factory);

            return handle;
        }

        public static string GetSchema(string connectionString, string dbType)
        {
            DbProviderFactory factory;
            DbConnection connection;
            
            try
            {
                factory = GetFactory(dbType);
            }
            catch (Exception exception)
            {
                throw new Exception("DB: " + dbType, exception);
            }

            connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            
            connection.Open();
            string result = connection.Database;
            connection.Close();
            
            return result;
        }

        private static DbProviderFactory GetFactory(string dbType)
        {
            DbProviderFactory factory;

            if (dbType == "System.Data.SQLite")
                factory = (DbProviderFactory)Activator.CreateInstance(Type.GetType("System.Data.SQLite.SQLiteFactory, System.Data.SQLite"));
            else
                factory = Microsoft.Data.SqlClient.SqlClientFactory.Instance;

            return factory;
        }

        public static void Open(IConnectionHandle handle)
        {
            ConnectionHandle connection;

            connection = (ConnectionHandle)handle;
            connection.Open();
        }

        public static void Close(IConnectionHandle handle)
        {
            ConnectionHandle connection;

            connection = (ConnectionHandle)handle;
            connection.Close();
        }

        public static ITransactionHandle BeginTransaction(IConnectionHandle handle)
        {
            ConnectionHandle connection;
            ITransactionHandle transaction;

            connection = (ConnectionHandle)handle;
            transaction = connection.BeginTransaction();

            return transaction;
        }

        public static void Commit(ITransactionHandle handle)
        {
            TransactionHandle transaction;

            transaction = (TransactionHandle)handle;
            transaction.Commit(); 
        }

        public static void Rollback(ITransactionHandle handle)
        {
            TransactionHandle transaction;

            transaction = (TransactionHandle)handle;
            transaction.RollBack();
        }


        public static DataTable ExecuteSql(IConnectionHandle handle, string query, Dictionary<string, object> namedParameters, params object[] parameterValues)
        {
            DataSet ds=ExecuteSqlWithDataSet(handle, query, namedParameters, parameterValues);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            return null;
        }
        public static DataTable ExecuteSql(IConnectionHandle handle, string query,CommandType commandType, Dictionary<string, object> namedParameters, params object[] parameterValues)
        {
            DataSet ds = ExecuteSqlWithDataSet(handle, query,commandType, namedParameters, parameterValues);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            return null;
        }

        public static DataSet ExecuteSqlWithDataSet(IConnectionHandle handle, string query, Dictionary<string, object> namedParameters, params object[] parameterValues)
        {
            ConnectionHandle connection;
            DbCommand command;            
            DbTransaction transaction = null;
            DataSet table = null;

            connection = (ConnectionHandle)handle;
            //Transaction kullanmadan sql çalýþtýrmak gerekebiliyor, bildiðim tek örnek "create database..." ihtiyacý...
            //Böyle bir ihtiyaç için direkt .DAL katmanýndaki metodlarý kullanmak gerekiyor.
            if (connection.TransactionHandle != null)
                transaction = ((TransactionHandle)connection.TransactionHandle).Transaction;

            using (DbDataAdapter adapter = connection.Factory.CreateDataAdapter())
            {
                table = new DataSet();
                
                command = connection.Factory.CreateCommand();
                command.CommandText = query;
                command.Connection = connection.Connection;
                command.Transaction = transaction;
                command.CommandTimeout = connection.TimeoutSeconds < 0 ? 30 : 0;

                command.Parameters.AddRange(CreateParameters(connection, namedParameters, parameterValues));

                adapter.SelectCommand = command;
                
                adapter.Fill(table);
            }

            return table;
        }
         
        public static DataSet ExecuteSqlWithDataSet(IConnectionHandle handle, string query,CommandType commandType, Dictionary<string, object> namedParameters, params object[] parameterValues)
        {
            ConnectionHandle connection;
            DbCommand command;
            DbTransaction transaction = null;
            DataSet table = null;

            connection = (ConnectionHandle)handle;
            //Transaction kullanmadan sql çalýþtýrmak gerekebiliyor, bildiðim tek örnek "create database..." ihtiyacý...
            //Böyle bir ihtiyaç için direkt .DAL katmanýndaki metodlarý kullanmak gerekiyor.
            if (connection.TransactionHandle != null)
                transaction = ((TransactionHandle)connection.TransactionHandle).Transaction;

            using (DbDataAdapter adapter = connection.Factory.CreateDataAdapter())
            {
                table = new DataSet();

                command = connection.Factory.CreateCommand();
                command.CommandType = commandType;
                command.CommandText = query;
                command.Connection = connection.Connection;
                command.Transaction = transaction;
                command.CommandTimeout = 0;

                command.Parameters.AddRange(CreateParameters(connection, namedParameters, parameterValues));

                adapter.SelectCommand = command;

                adapter.Fill(table);
            }

            return table;
        }

        public static int ExecuteNonQuery(IConnectionHandle handle, string query, Dictionary<string, object> namedParameters, object[] parameterValues, IParameterHandle[] extraParameters)
        {
            ConnectionHandle connection;
            DbTransaction transaction;
            DbCommand command;
            int numberOfRows;

            connection = (ConnectionHandle)handle;
            transaction = ((TransactionHandle)connection.TransactionHandle).Transaction;

            command = connection.Factory.CreateCommand();
            command.CommandText = query;
            command.Connection = connection.Connection;
            command.Transaction = transaction;

            command.CommandTimeout = connection.TimeoutSeconds < 0 ? 30 : 0;

            command.Parameters.AddRange(CreateParameters(connection, namedParameters, parameterValues));

            if(extraParameters!=null)
                foreach (ParameterHandle p in extraParameters)
                {
                    if (p == null) continue;
                    command.Parameters.Add(p.Parameter);
                }

            numberOfRows = command.ExecuteNonQuery();

            return numberOfRows;
        }

        public static object ExecuteScalar(IConnectionHandle handle, string query, Dictionary<string, object> namedParameters, params object[] parameterValues)
        {
            ConnectionHandle connection;
            DbTransaction transaction;
            DbCommand command;
            object result;

            connection = (ConnectionHandle)handle;
            transaction = ((TransactionHandle)connection.TransactionHandle).Transaction;

            command = connection.Factory.CreateCommand();
            command.CommandText = query;
            command.Connection = connection.Connection;
            command.Transaction = transaction;
            command.CommandTimeout = connection.TimeoutSeconds < 0 ? 30 : 0;

            command.Parameters.AddRange(CreateParameters(connection, namedParameters, parameterValues));

            result = command.ExecuteScalar();

            return result;
        }

        public static decimal ExecuteScalarD(IConnectionHandle handle, string query, Dictionary<string, object> namedParameters, params object[] parameterValues)
        {
            decimal result;

            result = Convert.ToDecimal(
                ExecuteScalar(handle, query, namedParameters, parameterValues));

            return result;
        }

        public static int ExecuteScalarI(IConnectionHandle handle, string query, Dictionary<string, object> namedParameters, params object[] parameterValues)
        {
            int result;

            result = Convert.ToInt32(
                ExecuteScalar(handle, query, namedParameters, parameterValues));

            return result;
        }

        public static long ExecuteScalarL(IConnectionHandle handle, string query, Dictionary<string, object> namedParameters, params object[] parameterValues)
        {
            long result;

            result = Convert.ToInt64(
                ExecuteScalar(handle, query, namedParameters, parameterValues));

            return result;
        }

        public static Dictionary<string,object> ExecuteNonQueryStoredProcedure(IConnectionHandle handle, string spName, params object[] parameterValues)
        {
            ConnectionHandle connection;
            DbTransaction transaction;
            DbCommand command;
            var namedParameters = new Dictionary<string, object>();
            var outputParameters = new Dictionary<string, object>();

            
            connection = (ConnectionHandle)handle;
            transaction = ((TransactionHandle)connection.TransactionHandle).Transaction;

            var dtPrm = ExecuteSql(connection, string.Format("select * from sys.parameters where object_id = object_id('dbo.{0}') order by parameter_id", spName), null);
            if (dtPrm != null)
            {
                var inputPrms = dtPrm.AsEnumerable().Where(r => Convert.ToBoolean(r["is_output"]) == false).ToList();
                if(inputPrms.Count != parameterValues.Length)
                {
                    throw new Exception("Stored Procedure input parametre sayýsý ile gönderdiðiniz parametre sayýsý birbirine uyuþmamaktadýr.\nSP Parametre Sayýsý:" + inputPrms.Count);
                }

                for (int i = 0; i < inputPrms.Count; i++)
                {
                    namedParameters.Add(inputPrms[i]["name"].ToString(), parameterValues[i]);
                }

                var outputPrms = dtPrm.AsEnumerable().Where(r => Convert.ToBoolean(r["is_output"]) == true).ToList();
                for (int i = 0; i < outputPrms.Count; i++)
                {
                    outputParameters.Add(outputPrms[i]["name"].ToString(), null);
                }
            }

            command = connection.Factory.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection.Connection;
            command.Transaction = transaction;
            command.CommandText = spName;

            command.CommandTimeout = connection.TimeoutSeconds < 0 ? 30 : 0;

            command.Parameters.AddRange(CreateParameters(connection, namedParameters, null));
            command.Parameters.AddRange(CreateParameters(connection, outputParameters, null, false));

            command.ExecuteNonQuery();

            foreach (DbParameter parameter in command.Parameters)
            {
                if(parameter.Direction == ParameterDirection.InputOutput)
                {
                    if(outputParameters.ContainsKey(parameter.ParameterName))
                    {
                        outputParameters[parameter.ParameterName] = parameter.Value;
                    }
                }
            }

            return outputParameters;
        }

        public static DataSet ExecuteStoredProcedure(IConnectionHandle handle, string spName, params object[] parameterValues)
        {
            ConnectionHandle connection;
            DbCommand command;
            DbTransaction transaction = null;
            DataSet table = null;
            var namedParameters = new Dictionary<string, object>();

            connection = (ConnectionHandle)handle;
            if (connection.TransactionHandle != null)
                transaction = ((TransactionHandle)connection.TransactionHandle).Transaction;

            var dtPrm = ExecuteSql(connection, string.Format("select * from sys.parameters where object_id = object_id('dbo.{0}') order by parameter_id", spName), null);
            if (dtPrm != null)
            {
                var inputPrms = dtPrm.AsEnumerable().Where(r => Convert.ToBoolean(r["is_output"]) == false).ToList();
                if (inputPrms.Count != parameterValues.Length)
                {
                    throw new Exception("Stored Procedure input parametre sayýsý ile gönderdiðiniz parametre sayýsý birbirine uyuþmamaktadýr.\nSP Parametre Sayýsý:" + inputPrms.Count);
                }

                for (int i = 0; i < inputPrms.Count; i++)
                {
                    namedParameters.Add(inputPrms[i]["name"].ToString(), parameterValues[i]);
                }
            }

            using (DbDataAdapter adapter = connection.Factory.CreateDataAdapter())
            {
                table = new DataSet();

                command = connection.Factory.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = spName;
                command.Connection = connection.Connection;
                command.Transaction = transaction;
                command.CommandTimeout = connection.TimeoutSeconds < 0 ? 30 : 0;

                command.Parameters.AddRange(CreateParameters(connection, namedParameters, null));

                adapter.SelectCommand = command;

                adapter.Fill(table);
            }

            return table;
        }

        public static DataTable MetaTableColumns(IConnectionHandle handle, string tablename)
        {
            ConnectionHandle connection;
            connection = (ConnectionHandle)handle;
            return connection.Connection.GetSchema("Columns", new string[] { null, null, tablename, null });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="name"></param>
        /// <param name="value">Must be not null!</param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static IParameterHandle NewParameter(string dbType, string name, object value, ParameterDirection direction)
        {
            DbProviderFactory factory;
            DbParameter parameter;
            IParameterHandle handle;

            factory = GetFactory(dbType);
            
            parameter = factory.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            parameter.Direction = direction;
            //parameter.DbType is automatically detected from type of value;

            handle = new ParameterHandle(parameter);

            return handle;
        }

        private static DbParameter[] CreateParameters(ConnectionHandle connection, Dictionary<string, object> namedParameters, object[] parameterValues, bool input=true)
        {
            DbParameter[] parameters = new DbParameter[0];

            parameterValues = parameterValues ?? new object[0];
            namedParameters = namedParameters ?? new Dictionary<string, object>();

            for (int i = 0; i < parameterValues.Length; i++)
            {
                namedParameters["prm" + i] = parameterValues[i];
            }

            int index = 0;
            if (parameterValues != null)
            {
                parameters = new DbParameter[namedParameters.Keys.Count];

                foreach (string key in namedParameters.Keys)
                {
                    object paramValue = namedParameters[key];

                    //Datetime mapping
                    if (paramValue is DateTime && DateTime.MinValue.Equals((DateTime)paramValue)) paramValue = DBNull.Value;
                    //null mapping
                    if (paramValue == null) paramValue = DBNull.Value;

                    DbParameter parameter = connection.Factory.CreateParameter();
                    parameter.ParameterName = key;
                    parameter.Value = paramValue;
                    if(!input)
                    {
                        parameter.Direction = ParameterDirection.InputOutput;
                        parameter.Size = 1000000;
                    }
                    
                    
                    parameters[index] = parameter;
                    index++;
                }
            }
            return parameters;
        }
    }
}