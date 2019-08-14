using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalaryCalc.Filters
{
    public class FilterConfirm: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            HttpContext ctx = HttpContext.Current;
            if (ctx.Session["SaleComfirmed"] == null)
            {
                filterContext.Result = new RedirectResult("~/sales/index");
                return;
            }
        }
    }
}