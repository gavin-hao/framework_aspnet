using Dongbo.OAuth.Core.Models;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.Extensions
{
    public static class OwinContextExtension
    {
        public static OwinOAuthContext GetOAuthContext(this IOwinContext context)
        {
            return new OwinOAuthContext(context);
        }
    }
}
