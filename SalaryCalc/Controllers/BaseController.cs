using SalaryCalc.Dal;
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
        private readonly DataContext db = new DataContext();
       
        public  User user { get; set; }

       public  List<UserRole> roles { get; set; }
        protected override void Initialize(RequestContext requestContext)
        {
             user = Session["LoggedUser"] as User;

            var roles = db.UserRoles.Where(w => w.PostionId == user.PostionId).ToList();
             

            base.Initialize(requestContext);
        }
    }
}