
using Microsoft.Owin;
using Owin;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Swashbuckle;
using Swashbuckle.Application;
using System.Web.Hosting;
using System.Web.Routing;
using System.Reflection;
using System.IO;
using System.Web.Http.Dispatcher;
using System;
using System.Web.Http.Description;
using System.Web.Http.Routing.Constraints;
using Swashbuckle.Swagger;
using System.Linq;
using WebApiHelpPage;
using System.Collections.Generic;
using System.Net.Http.Headers;
using WebApiHelpPageSelfHost;


/*namespace AxeSlide.Api
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            app.UseWelcomePage("/");


            app.UseErrorPage();

            var config = new HttpConfiguration();
            // 启用标记路由
            config.MapHttpAttributeRoutes();

            // 默认的 Web API 路由
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { controller = "Home", id = RouteParameter.Optional }
            );

            // There can be multiple exception loggers. (By default, no exception loggers are registered.)
            config.Services.Add(typeof(IExceptionLogger), new DongboExceptionLogger());
            //config.Filters.Add(new Api.Filters.ApiExceptionFilterAttribute());

            // There must be exactly one exception handler. (There is a default one that may be replaced.)
            // To make this sample easier to run in a browser, replace the default exception handler with one that sends
            // back text/plain content for all errors.
            //config.Services.Replace(typeof(IExceptionHandler), new GenericTextExceptionHandler());
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            HelpPageConfig.Register(config);
            // 将路由配置附加到 appBuilder
            app.UseWebApi(config);

            //EnableSwagger(GlobalConfiguration.Configuration);
        }
       

       
    }


}*/

