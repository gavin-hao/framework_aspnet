using System;
using System.Collections.Generic;
using System.Text;

namespace Dongbo.Data.Exceptions
{
	public class BadDataException : DataException
	{
		public BadDataException(string message) : base(message) { }

		public BadDataException(string message, string dbInstanceName)
			: base(message)
		{
			this.DBInstanceName = dbInstanceName;
		}

		public BadDataException(string message, string dbInstanceName, string procedureName)
			: base(message)
		{
			this.DBInstanceName = dbInstanceName;
			this.procedureName = procedureName;
		}
	}
}
