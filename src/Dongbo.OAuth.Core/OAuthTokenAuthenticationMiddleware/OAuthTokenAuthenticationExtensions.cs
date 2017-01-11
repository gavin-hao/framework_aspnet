using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Extensions;
namespace Dongbo.OAuth.Core.OAuthTokenAuthenticationMiddleware
{
    public static class OAuthTokenAuthenticationExtensions
    {
        public static IAppBuilder UseOAuthTokenAuthentication(this IAppBuilder app, OAuthTokenAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            app.Use(typeof(OAuthTokenAuthenticationMiddleware), app, options);
            app.UseStageMarker(PipelineStage.PostAuthenticate);
            return app;
        }
    }
    
}
