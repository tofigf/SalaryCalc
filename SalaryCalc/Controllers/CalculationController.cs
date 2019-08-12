using NCalc;
using SalaryCalc.Auth;
using SalaryCalc.Dal;
using SalaryCalc.Dtos;
using SalaryCalc.Filters;
using SalaryCalc.Models;
using SalaryCalc.Models.VwModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public ViewResult Index()
        {
            return View(model:db.CalcForums.ToList());
        }
        //Get [baseUrl]Calculation/CalcMethod
        [HttpGet]
        public ViewResult CalcMethod()
        {
            return View();
        }
        //Post [baseUrl]Calculation/CalcMethod
        [HttpPost]
        public ActionResult CalcMethod(CalcForum forum)
        {

            if (!ModelState.IsValid)
            {
                Session["Error"] = "Bütün xanaları doldurun";
                return RedirectToAction("index");
            }

            forum.Date = DateTime.Now;
            db.CalcForums.Add(forum);
            db.SaveChanges();
          
            return RedirectToAction("index");
        }
        //Get [baseUrl]Calculation/EditCalcMethod
        [HttpGet]
        public ActionResult EditCalcMethod(int? id)
        {
            if (id == null)
                return HttpNotFound();

            
            CalcForum forum = db.CalcForums.Find(id);
            if(forum != null)
            return View(model: forum);

            return RedirectToAction("index");
        }
        //Post [baseUrl]Calculation/EditCalcMethod
        [HttpPost]
        public ActionResult EditCalcMethod(CalcForum forum)
        {
            if(forum != null)
            {
                db.Entry(forum).State = EntityState.Modified;
                db.Entry(forum).Property(x => x.Date).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("index");
            }
            Session["Error"] = "Bütün xanaları doldurun";
            return RedirectToAction("index");
        }

        //Get [baseUrl]Calculation/CalculatedSalary
        [HttpGet]
        public ViewResult CalculatedSalary()
        {
            return View();
        }
        //Get [baseUrl]Calculation/CalculateSalary
        [HttpGet]
        public ViewResult CalculateSalary()
        {
            
            return View();
        }
        //Post [baseUrl]Calculation/CalculateSalary
        [HttpPost]
        public ActionResult CalculateSalary(DateTime Date, int[] usersId, bool all = false)
        {
           List<User> users = (List<User>)TempData["AllUsers"];
            if (all == false)
            {
                if (usersId != null)
                {
                    foreach (var id in usersId)
                    {
                        if (CheckCalculatedUsers(Date.Month, Date.Year, id))
                        {
                            Session["Error"] = "İstifadəçinin maaşı Hesablanmışdı";
                            return RedirectToAction("calculatesalary");
                        }
                        
                        User findedUser = db.Users.Find(id);
                        if (findedUser == null)
                            return Content("user empty");

                        //static keys
                        string bymonth = db.ButtonsStatics.FirstOrDefault(f => f.Key == "{ayliqgelir}").Key;
                        string byyear = db.ButtonsStatics.FirstOrDefault(f => f.Key == "{illikgelir}").Key;

                        string formula = findedUser.CalcForum.Formula;
                        if (ByMonth(id, Date.Month) == null)
                        {
                            Session["Error"] = "İşçinin seçilmiş ay üzrə heç bir satışı yoxdu";
                            return RedirectToAction("calculatesalary");
                        }
                        if (ByYear(id, Date.Year) == null)
                        {
                            Session["Error"] = "İşçinin seçilmiş il üzrə heç bir satışı yoxdu";
                            return RedirectToAction("calculatesalary");
                        }
                        string ByMonthValue = ByMonth(id, Date.Month).ToString();
                        string ByYearValue = ByYear(id, Date.Year).ToString();

                        if (formula.Contains(bymonth) || formula.Contains(byyear))
                        {
                            formula = formula.Replace(bymonth, ByMonthValue).Replace(byyear, ByYearValue);
                        }
                        //Expression
                        try
                        {
                            NCalc.Expression e = new NCalc.Expression(formula);
                            if (e.HasErrors())
                            {
                                {
                                    Session["Error"] = "Düstürda düzgün olmayan simvol var. Xəta!";
                                    return RedirectToAction("calculatesalary");
                                }
                            }
                            CalculatedSalaryByUser calculated = new CalculatedSalaryByUser
                            {

                                UserId = id,
                                Salary = (double)e.Evaluate(),
                                Date = Date
                            };

                            db.CalculatedSalaryByUsers.Add(calculated);
                            db.SaveChanges();


                        }
                        catch (EvaluationException e)
                        {
                           
                        }

                    }

                    return RedirectToAction("calculatedsalary");
                }
            }
            else if (all == true)
            {
                if (users != null)
                {
                    foreach (var userr in users)
                    {
                        if (CheckCalculatedUsers(Date.Month, Date.Year, userr.Id))
                        {
                            Session["Error"] = "İstifadəçinin maaşı Hesablanmışdı";
                            return RedirectToAction("calculatesalary");
                        }

                        //static keys
                        string bymonth = db.ButtonsStatics.FirstOrDefault(f => f.Key == "{ayliqgelir}").Key;
                        string byyear = db.ButtonsStatics.FirstOrDefault(f => f.Key == "{illikgelir}").Key;

                        string formula = userr.CalcForum.Formula;
                        if (ByMonth(userr.Id, Date.Month) == null)
                        {
                            Session["Error"] = "İşçinin seçilmiş ay üzrə heç bir satışı yoxdu";
                            return RedirectToAction("calculatesalary");
                        }
                        if (ByYear(userr.Id, Date.Year) == null)
                        {
                            Session["Error"] = "İşçinin seçilmiş il üzrə heç bir satışı yoxdu";
                            return RedirectToAction("calculatesalary");
                        }
                        string ByMonthValue = ByMonth(userr.Id, Date.Month).ToString();
                        string ByYearValue = ByYear(userr.Id, Date.Year).ToString();

                        if (formula.Contains(bymonth) || formula.Contains(byyear))
                        {
                            formula = formula.Replace(bymonth, ByMonthValue).Replace(byyear, ByYearValue);
                        }
                        //Expression
                        try
                        {
                            NCalc.Expression e = new NCalc.Expression(formula);
                            if (e.HasErrors())
                            {
                                Session["Error"] = "Düstürda düzgün olmayan simvol var. Xəta!";
                                return RedirectToAction("calculatesalary");
                            }
                            CalculatedSalaryByUser calculated = new CalculatedSalaryByUser
                            {

                                UserId = userr.Id,
                                Salary = (double)e.Evaluate(),
                                Date = Date
                            };

                            db.CalculatedSalaryByUsers.Add(calculated);
                            db.SaveChanges();


                        }
                        catch (EvaluationException e)
                        {
                           
                        }

                    }

                    return RedirectToAction("calculatedsalary");
                }
            }

            Session["Error"] = "Xəta!";
            return RedirectToAction("calculatesalary");

        }

        //Partial view
        [HttpGet]
        public PartialViewResult UsersForCalc(int? currMonth,int? currYear, int page = 1 )
        {
            int skip = ((int)page - 1) * 3;
          
            List<User> model = db.Users.Where(w=>w.CalculatedSalaryByUsers
            .Where(x => x.Date.Month == currMonth && x.Date.Year == currYear)
            .FirstOrDefault(a=>a.UserId == w.Id) == null).ToList();

            //Pagination list
            List<User> PaginateModel = db.Users.Where(w => w.CalculatedSalaryByUsers
            .Where(x => x.Date.Month == currMonth && x.Date.Year == currYear)
            .FirstOrDefault(a => a.UserId == w.Id) == null).OrderBy(a => a.Id)
            .Skip(skip).Take(3).ToList().ToList();
            ViewBag.TotalPage = Math.Ceiling(model.Count() / 3.0);
            ViewBag.Page = page;
            ViewBag.CurrMonth = currMonth;
            ViewBag.CurrYear = currYear;
            TempData["AllUsers"] = model;

            return PartialView("_UsersForCalcPartial", PaginateModel);
        }

        public double? ByMonth(int? id, int? currMonth)
        {
      
            if (db.Sales.FirstOrDefault(a => a.UserId == id) == null)
                return null;
            if (db.Sales.FirstOrDefault(a => a.UserId == id && a.Date.Month == currMonth) == null)
                return null;
            double? total = db.Sales.Where(w => w.UserId == id && w.Date.Month == currMonth).Sum(s=>s.Price);
            
            return total;
        }
        public double? ByYear(int? id, int? currYear)
        {
            if (db.Sales.FirstOrDefault(a => a.UserId == id) == null)
                return null;
            if (db.Sales.FirstOrDefault(a => a.UserId == id && a.Date.Year == currYear) == null)
                return null;
            double? total = db.Sales.Where(w => w.UserId == id && w.Date.Year == currYear).Sum(s => s.Price);
                      
            return total;
        }
        public bool CheckCalculatedUsers(int? currMonth, int? currYear ,int? userId)
        {
            if (db.CalculatedSalaryByUsers.FirstOrDefault(w => w.UserId == userId && w.Date.Month == currMonth && w.Date.Year == currYear) != null)
            {
                return true;
            }
            return false;
        }
       
    }
}