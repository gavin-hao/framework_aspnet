using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dongbo.WebExtension
{
    public class HttpForbiddenResult : HttpStatusCodeResult
    {
        public HttpForbiddenResult()
            : this(null)
        {
        }
        public HttpForbiddenResult(string statusDescription)
            : base(HttpStatusCode.Forbidden, statusDescription)
        {
        }
    }
}
