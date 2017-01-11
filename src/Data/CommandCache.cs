using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace Dongbo.Data
{
    class CommandCache : Dictionary<string, MySqlCommand>
    {
        internal MySqlCommand GetCommandCopy(MySqlConnection connection, string databaseInstanceName, string procedureName)
        {
            MySqlCommand copiedCommand;
            string commandCacheKey = databaseInstanceName + procedureName;

            if (!this.ContainsKey(commandCacheKey))
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = procedureName;

                if (connection.State != ConnectionState.Open)
                    connection.Open();
                MySqlCommandBuilder.DeriveParameters(command);
                connection.Close();

                lock (this)
                {
                    this[commandCacheKey] = command;
                }
            }

            copiedCommand = this[commandCacheKey].Clone();
            copiedCommand.Connection = connection;
            return copiedCommand;
        }


    }
    static class MySqlCommandExtensions
    {
        internal static MySqlCommand Clone(this MySqlCommand src)
        {
            var obj = new MySqlCommand(src.CommandText, src.Connection, src.Transaction);
            return obj;
        }
    }
}
