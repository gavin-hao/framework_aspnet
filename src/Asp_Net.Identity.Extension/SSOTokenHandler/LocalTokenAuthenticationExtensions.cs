using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{
    public static class LocalTokenAuthenticationExtensions
    {
        /// <summary>
        /// Adds a cookie-based authentication middleware to your web application pipeline.
        /// </summary>
        /// <param name="app">The IAppBuilder passed to your configuration method</param>
        /// <param name="options">An options class that controls the middleware behavior</param>
        /// <returns>The original app parameter</returns>
        public static IAppBuilder UseLocalTokenAuthentication(this IAppBuilder app, LocalTokenAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            app.Use(typeof(LocalTokenAuthenticationMiddleware), app, options);
            return app;
        }
    }
    
}
