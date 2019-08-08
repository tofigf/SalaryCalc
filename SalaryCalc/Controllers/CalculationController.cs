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
        //Get [baseUrl]Calculation/CalculatedSalary
        [HttpGet]
        public ActionResult CalculatedSalary()
        {
            return View();
        }
        //Get [baseUrl]Calculation/CalculateSalary
        [HttpGet]
        public ActionResult CalculateSalary()
        {
            ViewBag.User = db.Users.ToList();
            ViewBag.CalcForum = db.CalcForums.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult CalculateSalary(DateTime Date, int[] usersId)
        {
        
                foreach (var id in usersId)
                {
               if( db.CalculatedSalaryByUsers.Where(w => w.UserId == id).Any(a=>a.Date.Month == Date.Month || a.Date.Year == Date.Year))
                {
                    return Content("bu ay *** user maaş hesablanıb");
                }
                    if (user == null)
                        return Content("empty");
                    if (user != null)
                    {
                        //static keys
                        string bymonth = db.ButtonsStatics.Find(2).Key;
                        string byyear = db.ButtonsStatics.Find(3).Key;

                        string formula = user.CalcForum.Formula;
                        if (ByMonth(id, Date) == null || ByYear(id, Date) == null)
                            return Content("bu isniin hec bir satis yoxdu");
                        string ByMonthValue = ByMonth(id, Date).ToString();
                        string ByYearValue = ByYear(id, Date).ToString();

                        if (formula.Contains(bymonth) || formula.Contains(byyear))
                        {

                            formula = formula.Replace(bymonth, ByMonthValue).Replace(byyear, ByYearValue);
                        }

                        try
                        {
                            NCalc.Expression e = new NCalc.Expression(formula);
                            if (e.HasErrors())
                            {
                                return Content("expression error");
                            }
                            CalculatedSalaryByUser calculated = new CalculatedSalaryByUser
                            {

                                UserId = id,
                                Salary = (double)e.Evaluate(),
                                Date = DateTime.Now
                            };

                            db.CalculatedSalaryByUsers.Add(calculated);
                            db.SaveChanges();

                            return RedirectToAction("calculatedsalary");
                        }
                        catch (EvaluationException e)
                        {
                            return Content("catch");
                        }

                    
                }
            }

            return Content("500");
        }

        public double? ByMonth(int? id,DateTime? date)
        {
      
            if (db.Sales.Any(a => a.UserId != id && a.Date.Month != date.Value.Month))
                return null;
            double? total = db.Sales.Where(w => w.UserId == id && w.Date.Month == date.Value.Month).Sum(s=>s.Price);
            
            return total;
        }

            public double? ByYear(int? id, DateTime? date)
        {
            if (db.Sales.Any(a => a.UserId != id && a.Date.Year != date.Value.Year))
                return null;
            double? total = db.Sales.Where(w => w.UserId == id && w.Date.Year == date.Value.Year).Sum(s => s.Price);
                      
            return total;
        }
    }
}