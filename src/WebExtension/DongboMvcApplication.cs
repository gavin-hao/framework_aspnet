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
    public class CustomHttpHeader
    {
        public const string ErrorHandled = "x-dongbo-error-handled";
    }
    public class DongboMvcApplication : HttpApplication
    {

        protected virtual void Application_EndRequest(object sender, EventArgs e)
        {
            
            if (!Context.IsCustomErrorEnabled)
                return;

            var errorHandled = false;
            if (Context.Response.Headers[CustomHttpHeader.ErrorHandled] != null && (Context.Response.Headers[CustomHttpHeader.ErrorHandled] == "true"))
            {
                errorHandled = true;
                Context.Response.Headers.Remove(CustomHttpHeader.ErrorHandled);
            }
            if (errorHandled || Response.TrySkipIisCustomErrors)
                return;
           
            var statusCode = Context.Response.StatusCode;
            switch (statusCode)
            {
                case 404:
                    Response.ClearContent();
                    Response.ContentType = "html/text";
                    Response.TransmitFile("~/404.html");
                   
                    break;
                case 500:
                    Response.ClearContent();
                    Response.ContentType = "html/text";
                    Response.TransmitFile("~/500.html");
                    break;
                default:
                    Response.ClearContent();
                    Response.ContentType = "html/text";
                    Response.TransmitFile("~/500.html");
                    break;
            }
            // Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;
           

        }
        protected virtual void Application_Error(object sender, EventArgs e)
        {
            if (Context.IsCustomErrorEnabled)
            {
                GlobalErrorHandler.Instance.HandleError();
            }

        }
    }

}
