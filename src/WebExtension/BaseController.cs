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
    public class NotFoundModel
    {
        public string RequestedUrl { get; set; }
        public string ReferrerUrl { get; set; }
    }

    public class ErrorController : BaseController
    {
        public ActionResult PageNotFound(string url)
        {
            Response.StatusCode = 404;
            Response.ContentType = "text/html";

            ViewEngineResult result = FindErrorView("NotFound");
            if (null == result.View)
            {
                return Content("404 Not Found");
            }
            var model = new NotFoundModel();

            // If the url is relative ('NotFound' route) then replace with Requested path
            model.RequestedUrl = Request.Url.PathAndQuery;
            // Dont get the user stuck in a 'retry loop' by
            // allowing the Referrer to be the same as the Request
            model.ReferrerUrl = Request.UrlReferrer != null ? Request.UrlReferrer.OriginalString : "";
            //try
            //{
            //    this.ViewData = new ViewDataDictionary<NotFoundModel>(model);//.Add("Model", model);
            //    ViewContext viewContext = new ViewContext(ControllerContext, result.View, this.ViewData, this.TempData, Response.Output);
            //    result.View.Render(viewContext, viewContext.Writer);
            //}
            //finally
            //{
            //    result.ViewEngine.ReleaseView(ControllerContext, result.View);
            //}

            return View("NotFound", model);
        }
        private ViewEngineResult FindErrorView(string viewName)
        {
            //viewName=ControllerContext.RouteData.GetRequiredString("action");
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
            return result;
        }
        public ActionResult General(int? httpStatusCode, Exception error)
        {

            var code = (int)(this.ControllerContext.RouteData.Values["httpStatusCode"] ?? 500);
            Exception exception = this.ControllerContext.RouteData.Values["error"] as Exception;

            Response.StatusCode = httpStatusCode ?? code;
            Response.ContentType = "text/html";
            string originController = this.ControllerContext.RouteData.Values["originalController"] as string;
            string originAction = this.ControllerContext.RouteData.Values["originalAction"] as string;
            HandleErrorInfo model = new HandleErrorInfo(exception, originController, originAction);
            var viewResult = FindErrorView("GeneralError");
            if (viewResult == null)
                return Content(Response.StatusCode.ToString());
            return View("GeneralError", model);
        }

        public ActionResult Error()
        {
            var code = (int)(this.ControllerContext.RouteData.Values["httpStatusCode"] ?? 500);
            Exception exception = this.ControllerContext.RouteData.Values["error"] as Exception;

            Response.StatusCode = code;
            Response.ContentType = "text/html";
            string originController = this.ControllerContext.RouteData.Values["originalController"] as string;
            string originAction = this.ControllerContext.RouteData.Values["originalAction"] as string;
            HandleErrorInfo model = new HandleErrorInfo(exception, originController, originAction);
            var viewResult = FindErrorView("Error");
            if (viewResult == null)
                return Content("500");
            return View("Error", model);
        }
    }
    public class BaseController : Controller
    {


        public DateTime ServiceTime { get { return DateTime.Now; } }
        /// <summary>
        /// is debug mode ,when set web.config<compilation debug="true"> or request url /header contains debug=true,then return true
        /// </summary>
        protected bool IsDebuggingEnabled
        {
            get;
            private set;
        }

        public Dictionary<string, dynamic> DefaultPageInfo = new Dictionary<string, dynamic>();
       
        protected override void Initialize(RequestContext requestContext)
        {
            this.IsDebuggingEnabled = requestContext.HttpContext.IsCustomErrorEnabled ||
                requestContext.HttpContext.Request.Headers["debug"] == "true" ||
                requestContext.HttpContext.Request["debug"] == "true";
            base.Initialize(requestContext);
        }
       
        public HttpForbiddenResult HttpForbid()
        {
            return new HttpForbiddenResult("403 forbidden!");
        }
        public HttpForbiddenResult HttpForbid(string statusDescription)
        {
            return new HttpForbiddenResult(statusDescription);
        }
        public ActionResult ThrowIfHttpNotFound(string description)
        {
            throw new HttpException(404, description);
            return new EmptyResult();
        }

    }
}
