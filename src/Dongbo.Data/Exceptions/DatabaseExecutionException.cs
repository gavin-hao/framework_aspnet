using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using MySql.Data.MySqlClient;
using System.Security.Permissions;

namespace Dongbo.Data.Exceptions
{
	/// <summary>
	/// Thrown by Low level database access classes (Procedure class)
	/// </summary>
	[Serializable]
	public class DatabaseExecutionException : DataException
	{
		#region Database
		protected Database database;

		/// <summary>
		/// Affected database 
		/// </summary>
		public Database Database
		{
			get { return database; }
			set { database = value; }
		}
		#endregion

		#region Command
		private MySqlCommand command;

		public MySqlCommand Command
		{
			get { return command; }
			set { command = value; }
		}
		#endregion

		private static string GetParameterString(MySqlCommand c)
		{
			string ret = string.Empty;

			try
			{
				if (c == null) return string.Empty;

				foreach (MySqlParameter p in c.Parameters)
				{
					ret += "{?" + p.ParameterName + "}={" + p.Value + "}";
				}
			}
			catch (Exception) { } // Ignore these

			return ret;
		}

		public DatabaseExecutionException(Database database, string procedureName, Exception innerException)
			:
			base(
			"Database Exception Against Database: " + database.InstanceName + " \n "
			+ "\n With procedure: " + procedureName + " \n "
			+ "\n With inner exception message: " + innerException.Message,
			innerException
			)
		{
			this.database = database;
			this.procedureName = procedureName;
			this.innerException = innerException;
			this.instanceName = database.InstanceName;
			this.shortMessage =
				"Database Exception Against Database: " + database.InstanceName +
				"\n With inner exception message: " + innerException.Message;
		}

		public DatabaseExecutionException(Database database, string procedureName, MySqlCommand command, Exception innerException)
			:
			base(
			"Database Exception Against Database: " + database.InstanceName + " \n "
			+ "\n With procedure: " + procedureName + " \n "
			+ "\n With parameters: " + GetParameterString(command)
			+ "\n With inner exception message: " + innerException.Message,
			innerException
			)
		{
			this.database = database;
			this.procedureName = procedureName;
			this.command = command;
			this.innerException = innerException;
			this.instanceName = database.InstanceName;
			this.shortMessage =
				"Database Exception Against Database: " + database.InstanceName +
				"\n With inner exception message: " + innerException.Message;
		}

		public DatabaseExecutionException(Database database, string procedureName, string contextInfo, Exception innerException)
			:
			base(
			"Database Exception Against Database: " + database.InstanceName + " \n "
			+ "\n With procedure: " + procedureName + " \n "
			+ "\n With context info: " + contextInfo + " \n "
			+ "\n With inner exception message: " + innerException.Message,
			innerException
			)
		{
			this.database = database;
			this.procedureName = procedureName;
			this.innerException = innerException;
			this.instanceName = database.InstanceName;
			this.shortMessage =
				"Database Exception Against Database: " + database.InstanceName +
				"\n With inner exception message: " + innerException.Message;
		}

		#region Exception serialization support
		protected DatabaseExecutionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			// get the custom property out of the serialization stream and set the object's properties
			this.database = (Database)info.GetValue("Database", typeof(Database));
		}

		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// add the custom property into the serialization stream
			info.AddValue("Database", this.database);

			// call the base exception class to ensure proper serialization
			base.GetObjectData(info, context);
		}
		#endregion
	}
}
