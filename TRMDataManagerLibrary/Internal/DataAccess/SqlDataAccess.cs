using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDataManagerLibrary.Internal.DataAccess
{
    internal class SqlDataAccess : IDisposable  //internal class: can't be seen or used from anything outside the library. Nobody must talk directly to the database
    {
        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public List<T> LoadData<T,U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                //Query<T> is a method of Dapper, connects to db using connection string, a stored procedure and parameters of U type(generic type)
                List<T> rows = connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                
                connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                
            }
        }

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        //Open connection/start transaction method
        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        //Load using the transaction
        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
                List<T> rows = _connection.Query<T>(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();

                return rows;
        }

        //Save using the transaction
        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {
            _connection.Execute(storedProcedure, parameters,
                commandType: CommandType.StoredProcedure, transaction: _transaction);
        }

        //Close connection/stop a successuful transaction method
        public void CommitTransaction()
        {
            _transaction?.Commit();
            _connection?.Close();
        }

        //Close connection/stop a failed transaction
        public void RollBackTransaction()
        {
            _transaction?.Rollback(); //erases changes instead of commiting them
            _connection?.Close();
        }

        //Dispose
        public void Dispose()
        {
            CommitTransaction();
        }
        
    }
}
