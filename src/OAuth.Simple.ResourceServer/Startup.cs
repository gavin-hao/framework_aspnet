using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http.ExceptionHandling;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security;
using Dongbo.OAuth.Core;
using Dongbo.OAuth.Core.Providers;
using Dongbo.OAuth.Store;
using Microsoft.Owin.Extensions;
using Dongbo.AspNet.Identity.Extension;
using Dongbo.Owin.Security.AesDataProtectionProvider;
[assembly: OwinStartup(typeof(Dongbo.OAuth.Simple.ResourceServer.Startup))]

namespace Dongbo.OAuth.Simple.ResourceServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888


            app.UseWelcomePage("/");
           
            app.UseErrorPage();
            app.UseAesDataProtectionProvider();

            var oauthserOpt = new OAuthAuthorizationServerOptions();
            var option = new OAuthBearerAuthenticationOptions()
            {
                AccessTokenFormat = oauthserOpt.AccessTokenFormat,
                AccessTokenProvider = new AccessAuthenticationTokenProvider(new DongboAuthorizationServerOptions()
                {

                    //AccessTokenLifetime=
                    ClientManager = new ClientManager(new ClientStore()),
                    //IssuerUri = new Uri("/issuer"),
                    TokenManager = new TokenManager(new TokenStore()),
                    //UserManager = new OAuthUserManager<DongboUser>(new DongboUserManager(new UserStore<DongboUser>()))
                }),
                Provider = new OAuthBearerAuthenticationProvider()
                {
                    OnRequestToken = RequestToken
                },
                AuthenticationType = "OAuth"
            };
            app.UseOAuthBearerAuthentication(option);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            var config = new HttpConfiguration();
            // 启用标记路由
            config.MapHttpAttributeRoutes();

            // 默认的 Web API 路由
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            var cors = new System.Web.Http.Cors.EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            // There can be multiple exception loggers. (By default, no exception loggers are registered.)
            //config.Services.Add(typeof(IExceptionLogger), new DongboExceptionLogger());

            // There must be exactly one exception handler. (There is a default one that may be replaced.)
            // To make this sample easier to run in a browser, replace the default exception handler with one that sends
            // back text/plain content for all errors.
            //config.Services.Replace(typeof(IExceptionHandler), new GenericTextExceptionHandler());

            //regist help page route= '/help'
            //HelpPageConfig.Register(config);
            // 将路由配置附加到 appBuilder
            app.UseWebApi(config);


        }

        private Task RequestToken(OAuthRequestTokenContext context)
        {
            var s = context.Token;
            return Task.FromResult<object>(null);
        }
    }
}
