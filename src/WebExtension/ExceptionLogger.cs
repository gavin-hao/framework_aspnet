using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace Dongbo.AxeSlide.Api.Common
{
    /// <summary>
    /// global log exception
    /// </summary>
    public class DongboExceptionLogger : ExceptionLogger
    {
        private const string HttpContextBaseKey = "MS_HttpContext";

        private static readonly Logging.LogWrapper logger = new Logging.LogWrapper();

        public override void Log(ExceptionLoggerContext context)
        {
            // Retrieve the current HttpContext instance for this request.
            //HttpContext httpContext = GetHttpContext(context.Request);

            // Wrap the exception in an HttpUnhandledException so that ELMAH can capture the original error page.
            Exception exceptionToRaise = new HttpUnhandledException(message: null, innerException: context.Exception);
            // Send the exception to logger (for logging, mailing, filtering, etc.).
            logger.HandleException(exceptionToRaise, "Dongbo.AxeSlide.Api");
        }

        public override System.Threading.Tasks.Task LogAsync(ExceptionLoggerContext context, System.Threading.CancellationToken cancellationToken)
        {
            Exception exceptionToRaise = context.Exception;

            logger.HandleException(exceptionToRaise, "Dongbo.AxeSlide.Api");
            return Task.FromResult(0);
        }

        #region private method

        private static HttpContext GetHttpContext(HttpRequestMessage request)
        {
            HttpContextBase contextBase = GetHttpContextBase(request);

            if (contextBase == null)
            {
                return null;
            }

            return ToHttpContext(contextBase);
        }

        private static HttpContextBase GetHttpContextBase(HttpRequestMessage request)
        {
            if (request == null)
            {
                return null;
            }

            object value;

            if (!request.Properties.TryGetValue(HttpContextBaseKey, out value))
            {
                return null;
            }

            return value as HttpContextBase;
        }

        private static HttpContext ToHttpContext(HttpContextBase contextBase)
        {
            return contextBase.ApplicationInstance.Context;
        }

        #endregion private method
    }
}