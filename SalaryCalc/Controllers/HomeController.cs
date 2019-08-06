using SalaryCalc.Auth;
using SalaryCalc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalaryCalc.Controllers
{
    [FilterContext]
    [RolesAuth]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

  
    }
}