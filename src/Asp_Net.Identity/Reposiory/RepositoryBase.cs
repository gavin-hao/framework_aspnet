using Dongbo.ApiClient;
using Dongbo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Reposiory
{
    internal abstract class RepositoryBase
    {
        public ServiceClient ServiceClient { get; private set; }
        //public Database DB { get; private set; }
        public RepositoryBase()
            : this("DongboUser")
        {
        }
        public RepositoryBase(string serviceName = "DongboUser")
        {
            ServiceClient = ApiClientFactory.GetClient(serviceName);
        }
    }
}
