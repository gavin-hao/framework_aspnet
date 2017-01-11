using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.ApiClient
{
    public class ApiClientException : ApplicationException
    {
        public ApiClientException(string message) : base(message) { }
        public ApiClientException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        public ApiClientException(Exception innerException) : base("call api return a error",innerException) { }
    }
}
