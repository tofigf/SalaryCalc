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
    [FilterContext]
    [RolesAuth]
    public class CalculationController : BaseController
    {
        private readonly DataContext db = new DataContext();
        // GET: Calculation
        public ActionResult Index()
        {
            return View(model:db.CalcForums.ToList());
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
                return Content("required");

            forum.Date = DateTime.Now;
            db.CalcForums.Add(forum);
            db.SaveChanges();
          
            return RedirectToAction("index");
        }
        //Get [baseUrl]Calculation/CalculateSalary
        [HttpGet]
        public ActionResult CalculateSalary()
        {
            return View();
        }
        //[AllowAnonymous]
        public JsonResult CalculateSalary(int? UserId)
        {
            //buttonlarin idleri gelecek bura find edeceyik.
            if (UserId == null)
            {
                UserId = 3;
            }
            User user = db.Users.FirstOrDefault(f => f.Id == UserId);
            if (user != null)
            {

                string bymonth = db.ButtonsStatics.Find(2).Key;
                string byyear = db.ButtonsStatics.Find(3).Key;


                string formula = user.CalculatedSalaryByUsers.CalcForum.Formula;
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


            return Json("no", JsonRequestBehavior.AllowGet);
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