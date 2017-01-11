using Dongbo.OAuth.Core.Models.Http;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core
{
    public class UserInfoMiddleware : OwinMiddleware
    {
        /// <summary>
        /// Instantiates the middleware with an optional pointer to the next component.
        /// </summary>
        /// <param name="next">The next component.</param>
        public UserInfoMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        /// <summary>Process an individual request.</summary>
        /// <param name="context">The context.</param>
        /// <returns>A Task.</returns>
        public override async Task Invoke(IOwinContext context)
        {
            if (context.Authentication.User != null && context.Authentication.User.Identity.IsAuthenticated)
            {
                context.Response.ContentType = "application/json";
                var identity = new ClaimsIdentity(context.Authentication.User.Identity);

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new IdentityResponse(identity.Claims.ToArray())));
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";
                context.Response.Headers["WWW-Authenticate"] = string.Empty;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ErrorResponse(Constants.ErrorCode.InvalidToken)));
            }
        }
    }
}
