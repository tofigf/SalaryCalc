using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalaryCalc.Controllers
{
    public class CalculationController : Controller
    {
        // GET: Calculation
        public ActionResult Index()
        {
            return View();
        }
    }
}