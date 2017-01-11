using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using Dongbo.Data.MapperDelegates;
using Dongbo.Logging;

namespace Dongbo.Data
{
   

    /// <summary>
    /// Wraps the execution of a stored procedure.
    /// </summary>
    public static class Procedure
    {
        /// <summary>
        /// Executes and returns an open IRecordSet, which encapsulates an OPEN DATAREADER.  DISPOSE IN FINALLY CLAUSE.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="procedureName"></param>
        /// <param name="parameterMapper"></param>
        /// <returns></returns>
        /// <param name="objectInstance"></param>
        public static IRecordSet Execute<T>(Database database, string procedureName, 
            ParameterMapper<T> parameterMapper, T objectInstance)
        {

            MySqlConnection connection = database.GetConnection();
            MySqlCommand command = CommandFactory.CreateParameterMappedCommand<T>(
                connection, database.InstanceName, procedureName, parameterMapper, objectInstance);

            if (log.IsDebugEnabled)
                log.MethodDebugFormat(LOG_PREFIX, "Database: {0}, Procedure: {1}, Parameters: {2}", database.InstanceName, procedureName, DebugUtil.GetParameterString(command));

            try
            {

                command.Connection.Open();
                IRecordSet record = new DataRecord(command.ExecuteReader(CommandBehavior.CloseConnection));
                return record;
            }
            catch (Exception exc)
            {
                command.Connection.Close();

                throw new Dongbo.Data.Exceptions.DatabaseExecutionException(database, procedureName, command, exc);

            }
        }

        /// <summary>
        /// Local instance of LogWrapper
        /// </summary>
        private static readonly LogWrapper log = new LogWrapper();

        private const string LOG_PREFIX = "DB_CALL_LOG - Procedure";

        /// <summary>
        /// Executes and returns an open IRecordSet, which encapsulates an OPEN DATAREADER.  DISPOSE IN FINALLY CLAUSE.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="procedureName"></param>
        /// <param name="parameterMapper"></param>
        /// <returns></returns>
        public static IRecordSet Execute(Database database, string procedureName, ParameterMapper parameterMapper)
        {

            MySqlConnection connection = database.GetConnection();
			MySqlCommand command = CommandFactory.CreateParameterMappedCommand(connection, database.InstanceName, procedureName, parameterMapper);

            if (log.IsDebugEnabled)
                log.MethodDebugFormat(LOG_PREFIX, "Database: {0}, Procedure: {1}, Parameters: {2}", database.InstanceName, procedureName, DebugUtil.GetParameterString(command));

            try
            {

                command.Connection.Open();
                IRecordSet record = new DataRecord(command.ExecuteReader(CommandBehavior.CloseConnection));
                return record;
            }
            catch(Exception exc)
            {
                command.Connection.Close();

                throw new Dongbo.Data.Exceptions.DatabaseExecutionException(database, procedureName, command, exc);

            }
        }

        /// <summary>
        /// Executes and returns an open IRecordSet, which encapsulates an OPEN DATAREADER.  DISPOSE IN FINALLY CLAUSE.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IRecordSet Execute(Database database, string procedureName, params object[] parameters)
        {

            IRecordSet recordSet;



            MySqlConnection connection = database.GetConnection();

			MySqlCommand command = CommandFactory.CreateCommand(connection, database.InstanceName, procedureName, parameters);
            try
            {

                connection.Open();
                recordSet = new DataRecord(command.ExecuteReader(CommandBehavior.CloseConnection));
                return recordSet;
            }
            catch(Exception exc)
            {
                connection.Close();

                throw new Dongbo.Data.Exceptions.DatabaseExecutionException(database, procedureName, command, exc);
            }
        }
        /// <summary>
        /// wzg添加
        /// </summary>
        /// <param name="database"></param>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IRecordSet Execute(Database database, string procedureName, MySqlParameter[] parameters)
        {

            IRecordSet recordSet;
            MySqlConnection connection = database.GetConnection();

            
            MySqlCommand command = CommandFactory.CreateCommand(connection, database.InstanceName, procedureName, parameters);
            
            try
            {

                connection.Open();
                recordSet = new DataRecord(command.ExecuteReader(CommandBehavior.CloseConnection));
                return recordSet;
            }
            catch (Exception exc)
            {
                connection.Close();

                throw new Dongbo.Data.Exceptions.DatabaseExecutionException(database, procedureName, command, exc);
            }
        }

    
        /// <summary>
        /// Assembly-scoped class for returning a DataReader
        /// </summary>
        /// <param name="database"></param>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal static IDataReader ExecuteReader(Database database, string procedureName, params object[] parameters)
        {
            IDataReader reader;

            MySqlConnection connection = database.GetConnection();
			MySqlCommand command = CommandFactory.CreateCommand(connection, database.InstanceName, procedureName, parameters);

            try
            {

                connection.Open();
                reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch(Exception exc)
            {
                connection.Close();

                throw new Dongbo.Data.Exceptions.DatabaseExecutionException(database, procedureName, command, exc);
            }
        }


        /// <summary>
        /// Assembly-scoped class for returning a DataReader.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="procedureName"></param>
        /// <param name="parameterMapper"></param>
        /// <returns></returns>
           internal static IDataReader ExecuteReader(Database database, string procedureName, ParameterMapper parameterMapper)
           {

               MySqlConnection connection = database.GetConnection();
			   MySqlCommand command = CommandFactory.CreateParameterMappedCommand(connection, database.InstanceName, procedureName, parameterMapper);

               if (log.IsDebugEnabled)
                   log.MethodDebugFormat(LOG_PREFIX, "Database: {0}, Procedure: {1}, Parameters: {2}", database.InstanceName, procedureName, DebugUtil.GetParameterString(command));

               try
               {

                   command.Connection.Open();
                   IDataReader record = command.ExecuteReader(CommandBehavior.CloseConnection);
                   return record;
               }
               catch(Exception exc)
               {
                   command.Connection.Close();

                   throw new Dongbo.Data.Exceptions.DatabaseExecutionException(database, procedureName, command, exc);
               }
           }
    }
}
