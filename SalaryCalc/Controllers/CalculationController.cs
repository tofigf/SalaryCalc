using NCalc;
using SalaryCalc.Auth;
using SalaryCalc.Dal;
using SalaryCalc.Filters;
using SalaryCalc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SalaryCalc.Controllers
{
  //  [FilterContext]
  //  [RolesAuth]
    public class CalculationController : Controller
    {
        private readonly DataContext db = new DataContext();
        // GET: Calculation
        public ActionResult Index()
        {
            return View();
        }
        //Get [baseUrl]Calculation/CalcMethod
        [HttpGet]
        public ActionResult CalcMethod()
        {
            return View();
        }
        //Post [baseUrl]Calculation/CalcMethod
        [HttpPost]
        public ActionResult CalcMethod(CalcForum forum)
        {
           
            if (!ModelState.IsValid)
                return Content("requierd");
            foreach (var match in Regex.Matches(forum.Formula, @"([*+/\-)(])|([0-9]+)"))
            {
                
            }

            forum.Date = DateTime.Now;
            db.CalcForums.Add(forum);
            db.SaveChanges();
            //{CurrentEmployee.TOTALSALESBYMONTH} * 30) / 100]
          
            return Content("error");
        }

        [AllowAnonymous]
        public JsonResult CalculateSalary(int? UserId)
        {
         
            if(UserId == null)
            {
                UserId = 3;
            }
            User user = db.Users.FirstOrDefault(f => f.Id == UserId);
            if (user != null)
            {
                string bymonth = db.ButtonsStatics.Find(2).Key;
                string byyear = db.ButtonsStatics.Find(3).Key;


          
                string formula = user.CalcForum.Formula;
                string ByMonthValue = ByMonth(user).ToString();
                string ByYearValue = ByYear(user).ToString();

                if (formula.Contains(bymonth) || formula.Contains(byyear))
                {
                 
                   formula = formula.Replace(bymonth, ByMonthValue).Replace(byyear, ByYearValue);
                }

                try
                {
                    NCalc.Expression e = new NCalc.Expression(formula);
                    if (!e.HasErrors())
                        return Json(e.Evaluate().ToString(), JsonRequestBehavior.AllowGet);
         
                }
                catch (EvaluationException e)
                {
                    return Json("no", JsonRequestBehavior.AllowGet);
                }

            }
       

            return Json("no" , JsonRequestBehavior.AllowGet);
        }

        public double ByMonth(User user)
        {
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;
            double total = db.Sales.Where(w => w.UserId == user.Id).Sum(s=>s.Price);
            return total;
        }

        public double ByYear(User user)
        {
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;
            double total = db.Sales.Where(w => w.UserId == user.Id).Sum(s => s.Price);
            return total;
        }
    }
}