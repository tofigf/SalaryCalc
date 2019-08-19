using SalaryCalc.Dal;
using SalaryCalc.Filters;
using SalaryCalc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SalaryCalc.Controllers
{
  
    public abstract class BaseController : Controller
    {
        protected  User userLoginned { get; set; }
        protected  DataContext db { get; set; }

        public  List<UserRole> roles { get; set; }
        protected override void Initialize(RequestContext requestContext)
        {
             userLoginned = requestContext.HttpContext.Session["LoggedUser"] as User;
             db = new DataContext();
            if (userLoginned == null)
            {
                roles = db.UserRoles.Where(w => w.PostionId == db.Postions.FirstOrDefault().Id).ToList();
            }
            else
            {
                roles = db.UserRoles.Where(w => w.PostionId == userLoginned.PostionId).ToList();

            }
            base.Initialize(requestContext);
        }
    }
}