using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Owin.Security.AesDataProtectionProvider
{
    public static class AppBuilderExtensions
    {
        const string hostAppNameKey = "host.AppName";
        const string defaultKey = "axeslide:security:dataprotector:key@2016";
        public static void UseAesDataProtectionProvider(this IAppBuilder app)
        {

            UseAesDataProtectionProvider(app, defaultKey);
        }
        public static void UseAesDataProtectionProvider(this IAppBuilder app, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (app.Properties.ContainsKey(hostAppNameKey))
                {
                    key = app.Properties[hostAppNameKey].ToString();
                }
            }

            app.SetDataProtectionProvider(new AesDataProtectionProvider(key));
        }


    }
}
