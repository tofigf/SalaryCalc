using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SalaryCalc.Filters
{
    public class FilterContext: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
        
            HttpContext ctx = HttpContext.Current;
            if (ctx.Session["UserId"] == null)
            {
                filterContext.Result = new RedirectResult("~/home/index");
            }
        }
    }
}