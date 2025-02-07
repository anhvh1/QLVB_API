using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Com.Gosol.QLVB.Ultilities;

namespace Workflow.DatabaseProxy
{
    public class DatabaseProxy
    {
        //private static string _connectionString = ConfigurationSettings.AppSettings["BackendDB"];
        private static string _connectionString = SQLHelper.appConnectionStrings;
        private static string canNotCreateClass = "CanNotCreateClass";
        private static string databaseException = "DatabaseException";
        private static string noCommandText = "NoCommandText";
        private static string noConnectionString = "NoConnectionString";
        private static string noParamName = "NoParameterName";

        protected DatabaseProxy()
        {
            throw new Exception("cannot create class");
        }

        public static IDataParameter AddParameter(IDbCommand dbCommand, string paramName, int paramValue)
        {
            if ((paramName == null) || (paramName.Length <= 0))
            {
                throw new Exception("no parameter name");
            }
            IDbDataParameter parameter = dbCommand.CreateParameter();
            parameter.ParameterName = paramName;
            parameter.Value = paramValue;
            dbCommand.Parameters.Add(parameter);
            return parameter;
        }

        public static IDataParameter AddParameter(IDbCommand dbCommand, string paramName, string paramValue)
        {
            IDbDataParameter parameter = dbCommand.CreateParameter();
            parameter.ParameterName = paramName;
            parameter.Value = paramValue;
            dbCommand.Parameters.Add(parameter);
            return parameter;
        }

        public static IDataParameter AddParameter(IDbCommand dbCommand, string paramName, DateTime paramValue)
        {
            IDbDataParameter parameter = dbCommand.CreateParameter();
            parameter.ParameterName = paramName;
            parameter.Value = paramValue;
            dbCommand.Parameters.Add(parameter);
            return parameter;
        }
        public static IDataParameter AddParameter(IDbCommand dbCommand, string paramName, DBNull paramValue)
        {
            IDbDataParameter parameter = dbCommand.CreateParameter();
            parameter.ParameterName = paramName;
            parameter.Value = paramValue;
            dbCommand.Parameters.Add(parameter);
            return parameter;
        }

        public static IDbCommand CreateDBCommand(IDbConnection dbConnection, string commandText)
        {
            if ((commandText == null) || (commandText.Length <= 0))
            {
                throw new Exception("No Command Text");
            }
            IDbCommand command = dbConnection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = CommandType.Text;
            return command;
        }

        public static IDbConnection CreateDBConnection()
        {
            if ((_connectionString == null) || (_connectionString.Length <= 0))
            {
                throw new Exception("Cannot create connection");
            }
            return new SqlConnection(_connectionString);
        }
    }
}
