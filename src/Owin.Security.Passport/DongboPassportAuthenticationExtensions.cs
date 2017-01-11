using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Extensions;

namespace Dongbo.Owin.Security.Passport
{
    public static class DongboPassportAuthenticationExtensions
    {
        public static IAppBuilder UseDongboPassportAuthentication(this IAppBuilder app, string appId)
        {
            return UseDongboPassportAuthentication(app, new DongboPassportAuthenticationOptions() { AppId = appId });
        }
        public static IAppBuilder UseDongboPassportAuthentication(this IAppBuilder app, DongboPassportAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            app.Use(typeof(DongboPassportAuthenticationMiddleware), app, options);
            return app;
        }
    }
}
