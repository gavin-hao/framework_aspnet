using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
namespace Dongbo.WebExtension
{
    public static class HtmlHelperExtensions
    {

        /// <summary>
        /// Returns an anchor element (a element) that contains the virtual path of the specified action.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="accountTypeRequired">The required account type.</param>
        /// <returns>The anchor element (a element) that contains the virtual path of the specified action.</returns>
        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper,
            string linkText,
            string actionName,
            string controllerName,
            string roles)
        {
            MvcHtmlString link = MvcHtmlString.Empty;
            var auth = new RoleAuthorizeAttribute();
            auth.Roles = roles;
            var visible = auth.PerformAuthorizeCore(htmlHelper.ViewContext.HttpContext);
            if (visible)
                link = htmlHelper.ActionLink(linkText, actionName, controllerName);
            return link;
        }
    }
}
