﻿using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using Dongbo.Data.MapperDelegates;
using System.Data;
using Dongbo.Data.Exceptions;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Dongbo.Logging;

namespace Dongbo.Data
{
    internal class CommandFactory
    {
        private static CommandCache commandCache = new CommandCache();

		/// <summary>
		/// Basically, all non-ascii characters.  We're banning anything above 255 in non-unicode fields
		/// </summary>
		private static Regex forbiddenVarchars = new Regex("[^\u0009-\u00FF]", RegexOptions.Compiled);  //private static Regex forbiddenVarchars = new Regex("[\u0100-\u0180\u019F-\u01A1\u0197\u019A\u01AB\u01AE-\u01B0\u01B6\u01CD-\u01F0\u0261\u02B9-\u02C8\u030E\u0393\u0398\u03A3\u03A6\u03A9\u03B1\u03B4\u03B5\u03C0\u03C3\u03C4\u03C6\u04BB\u2017\u2032\u2044\u207F\u20A7\u2102\u2107\u210A-\u2134\u2215\u221A\u2229\u2261\u2264\u2265\u2320\u2321\u2329\u232A\u3008\u3009\uFF02-\uFF09\uFF0F\uFF1C\uFF1D\uFF1E\u0268\u0275\u0288\u03B3\u03B8\u03C9]", RegexOptions.Compiled);
		private static readonly string forbiddenVarcharsReplacement = "?";
        private static readonly string parameterPrefix="@";
		private static void MapParameters(MySqlCommand command, object[] parameters)
		{
			//int returnValueOffset = 1;
			if (parameters == null)
			{
				AssertParameterCount(command.Parameters.Count, 0, 0, command.CommandText);
				return;
			}
			else
				AssertParameterCount(command.Parameters.Count, parameters.Length, 0, command.CommandText);

			for ( int i = 0 , j = parameters.Length-1; i <= j; i++ )
			{
				object parameterValue = parameters[i];
				if (parameterValue == null)
					parameterValue = DBNull.Value;
				command.Parameters[i].Value = parameterValue;
			}
		}



        private static void MapParameters(MySqlCommand command, StoredProcedureParameterList parameters)
		{
			//int returnValueOffset = 1;

			int parameterCount = parameters.Count;
			foreach (StoredProcedureParameter spp in parameters)
			{
				if (spp.ParameterDirection == ParameterDirectionWrap.ReturnValue)
				{
					parameterCount--;
					break;
				}
			}

			AssertParameterCount(command.Parameters.Count, parameterCount, 0, command.CommandText);

			for (int i = 0 , j = parameters.Count-1; i <= j; i++)
			{
				StoredProcedureParameter spp = parameters[i];
				MySqlParameter sqlParameter;
				if (spp.Key != null)
					sqlParameter = command.Parameters[spp.Key];
				else
					sqlParameter = command.Parameters[i];

				sqlParameter.Value = spp.Value;
				if (sqlParameter.Value == null)
					sqlParameter.Value = DBNull.Value;

				switch (spp.ParameterDirection)
				{
					case ParameterDirectionWrap.Input:
						sqlParameter.Direction = ParameterDirection.Input;
						break;
					case ParameterDirectionWrap.Output:
						sqlParameter.Direction = ParameterDirection.Output;
						break;
					case ParameterDirectionWrap.InputOutput:
						sqlParameter.Direction = ParameterDirection.InputOutput;
						break;
					case ParameterDirectionWrap.ReturnValue:
						sqlParameter.Direction = ParameterDirection.ReturnValue;
						break;
					default:
						throw new ArgumentException("Unknow parameter direction specified: " + spp.ParameterDirection.ToString());
				}

				if (spp.Size.HasValue)
					sqlParameter.Size = spp.Size.Value;
			}
		}

		private static void AssertParameterCount(int numProcedureParameters, int numPassedParameters, int returnValueOffset, string procedureName) 
		{
// putting this back as compiler directives because
// the assertion should be for local test code only
#if DEBUG
                if (numProcedureParameters != numPassedParameters + returnValueOffset)
                    throw new ArgumentException(string.Format("The incorrect number of parameters were supplied to the procedure {0}.  The number supplied was: {1}.  The number expected is: {2}.",
                        procedureName, numPassedParameters, numProcedureParameters - returnValueOffset));
#else
                if (numProcedureParameters < numPassedParameters + returnValueOffset)
                    throw new ArgumentException(string.Format("Too many parameters parameters were supplied to the procedure {0}.  The number supplied was: {1}.  The number expected is: {2}.",
                        procedureName, numPassedParameters, numProcedureParameters - returnValueOffset));
#endif
		}

        internal static MySqlCommand CreateParameterizedCommand(MySqlConnection connection, string databaseInstanceName, string commandName)
        {
            
			MySqlCommand command = commandCache.GetCommandCopy(connection, databaseInstanceName, commandName);
			if (log.IsMoreDebugEnabled)
				log.MoreDebug(new StackFrame(1, true), command);
			return command;

        }

        /// <summary>
        /// Creates and prepares MySqlCommand object and calls parameterMapper to populate command parameters
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="procedureName"></param>
        /// <param name="parameterMapper"></param>
        /// <returns></returns>
		internal static MySqlCommand CreateParameterMappedCommand(MySqlConnection connection, string databaseInstanceName, string procedureName, ParameterMapper parameterMapper)
        {
           
			MySqlCommand command = connection.CreateCommand();
			command.CommandText = procedureName;
			command.CommandType = CommandType.StoredProcedure;

            if(parameterMapper != null)
            {
                ParameterSet pSet = new ParameterSet(command.Parameters);
                parameterMapper(pSet);
            }

			ApplySecurity(command, commandCache.GetCommandCopy(connection, databaseInstanceName, procedureName).Parameters);

			if (log.IsMoreDebugEnabled)
				log.MoreDebug(new StackFrame(1, true), command);
            return command;
        }

        /// <summary>
        /// Creates and prepares MySqlCommand object and calls strongly typed parameterMapper to populate command parameters
        /// </summary>
		internal static MySqlCommand CreateParameterMappedCommand<T>(MySqlConnection connection, string databaseInstanceName, string procedureName, ParameterMapper<T> parameterMapper, T objectInstance)
        {
           

            MySqlCommand command = connection.CreateCommand();
			command.CommandText = procedureName;
			command.CommandType = CommandType.StoredProcedure;

            if (parameterMapper != null)
            {
                ParameterSet pSet = new ParameterSet(command.Parameters);
                parameterMapper(pSet, objectInstance);
            }

			ApplySecurity(command, commandCache.GetCommandCopy(connection, databaseInstanceName, procedureName).Parameters);

			if (log.IsMoreDebugEnabled)
				log.MoreDebug(new StackFrame(1, true), command);
            return command;
        }

        internal static MySqlCommand CreateCommand(MySqlConnection connection, string databaseInstanceName, string commandName, params object[] parameterValues)
		{
            MySqlCommand command = CreateParameterizedCommand(connection, databaseInstanceName, commandName);
			MapParameters(command, parameterValues);
			ApplySecurity(command, commandCache.GetCommandCopy(connection, databaseInstanceName, commandName).Parameters);

			if (log.IsMoreDebugEnabled)
				log.MoreDebug(new StackFrame(1, true), command);
			return command;
		}

        internal static MySqlCommand CreateCommand(MySqlConnection connection, string databaseInstanceName, string commandName, MySqlParameter[] parameterValues)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = commandName;
            command.CommandType = CommandType.StoredProcedure;

            foreach (MySqlParameter p in parameterValues)
            {
                command.Parameters.Add(p);
            }
            AssertParameterCount( commandCache.GetCommandCopy(connection, databaseInstanceName, commandName).Parameters.Count,command.Parameters.Count, 1, command.CommandText);
            
            ApplySecurity(command, commandCache.GetCommandCopy(connection, databaseInstanceName, commandName).Parameters);

            if (log.IsMoreDebugEnabled)
                log.MoreDebug(new StackFrame(1, true), command);
            return command;
        }


		/// <summary>
		/// Creates and prepares an MySqlCommand object and sets parameters from the parameter list either by their index value or name.
		/// </summary>
		/// <returns></returns>
		internal static MySqlCommand CreateCommand(MySqlConnection connection, string databaseInstanceName, string commandName, StoredProcedureParameterList parameterList)
		{
            MySqlCommand command = CreateParameterizedCommand(connection, databaseInstanceName, commandName);
			MapParameters(command, parameterList);
			ApplySecurity(command, commandCache.GetCommandCopy(connection, databaseInstanceName, commandName).Parameters);

			if (log.IsMoreDebugEnabled)
				log.MoreDebug(new StackFrame(1, true), command);
			return command;
		}

		private static readonly LogWrapper log = new LogWrapper();

		#region Helper Methods
        private static void ApplySecurity(MySqlCommand command, MySqlParameterCollection parameterTypes)
		{
			
				foreach (MySqlParameter parameter in parameterTypes)
				{
					if (parameter.DbType == DbType.AnsiString)
					{
                        string parameterName = parameter.ParameterName.Replace(parameterPrefix, "").ToLower();
						foreach (MySqlParameter commandParameter in command.Parameters)
						{
                            if ((commandParameter.ParameterName.Replace(parameterPrefix, "").ToLower() == parameterName) &&
								((commandParameter.Value != null) && (commandParameter.Value != DBNull.Value)))
							{
								commandParameter.Value = forbiddenVarchars.Replace(commandParameter.Value.ToString(), forbiddenVarcharsReplacement);
							}
						}
					}
				}
			
		}
		#endregion
    }
}
