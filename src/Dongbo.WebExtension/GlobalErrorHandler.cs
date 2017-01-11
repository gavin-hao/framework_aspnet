using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace Dongbo.WebExtension
{
    public class GlobalErrorHandler
    {
        private GlobalErrorHandler() { }
        private static GlobalErrorHandler instance = new GlobalErrorHandler();
        public static GlobalErrorHandler Instance { get { return instance; } }

        private static readonly Logging.LogWrapper logger = new Logging.LogWrapper();
        protected HttpContext Context
        {
            get { return HttpContext.Current; }
        }
        protected HttpServerUtility Server
        {
            get
            {
                return Context.Server;
            }
        }
        protected HttpRequest Request
        {
            get
            {
                return Context.Request;
            }
        }
        protected HttpResponse Response
        {
            get
            {
                return Context.Response;
            }
        }


        public void HandleError()
        {

            if (HttpContext.Current == null)
                return;
            if (!Context.IsCustomErrorEnabled)
                return;
            Exception exception = Server.GetLastError();

            HttpException httpException = exception as HttpException;

            if (httpException == null)
                return;
            string controllerName = (string)Request.RequestContext.RouteData.Values["controller"];
            string actionName = (string)Request.RequestContext.RouteData.Values["action"];

            Response.Clear();
            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");


            Response.StatusCode = httpException.GetHttpCode();
            if (Response.StatusCode >= 500)//log the http error
            {
                logger.Error("an internal server exception occurred", exception);
            }
            switch (Response.StatusCode)
            {
                case 404:
                    routeData.Values.Add("action", "PageNotFound");
                    break;
                case 500:
                    // Server error.
                    routeData.Values.Add("action", "Error");
                    break;
                default:
                    routeData.Values.Add("action", "General");
                    routeData.Values.Add("httpStatusCode", Response.StatusCode);
                    break;
            }
            routeData.Values.Add("error", exception);
            routeData.Values.Add("originalController", controllerName);
            routeData.Values.Add("originalAction", actionName);
            //Context.Response.Headers.Add(CustomHttpHeader.ErrorHandled, "true");
            // Clear the error on server.
            Server.ClearError();
            // Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;

            // Call target Controller and pass the routeData.
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(
                 new HttpContextWrapper(Context), routeData));



        }


    }
}
