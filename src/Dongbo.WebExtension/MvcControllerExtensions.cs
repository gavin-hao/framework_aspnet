using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dongbo.WebExtension
{
    public static class MvcControllerExtensions
    {
        public static ActionResult ThrowHttpNotFound(this Controller controller, string description)
        {
            throw new HttpException(404, description);
            //return new EmptyResult();
        }
        public static ActionResult ThrowHttpNotFound(this Controller controller)
        {
            throw new HttpException(404, "NOT FOUND");
            //return new EmptyResult();
        }
    }
}
