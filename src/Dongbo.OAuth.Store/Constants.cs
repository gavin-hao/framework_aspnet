using Dongbo.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Store
{
    internal static class StoreConstants
    {
        public static ServiceClient OAuthClientServiceClient
        {
            get
            {
                return ApiClientFactory.GetClient("OAuthService");

            }
        }
        public static ServiceClient TokenServiceClient
        {
            get
            {
                return ApiClientFactory.GetClient("OAuthService");

            }
        }
    }
}
