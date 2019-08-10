﻿using NCalc;
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
                return Content("required");

            forum.Date = DateTime.Now;
            db.CalcForums.Add(forum);
            db.SaveChanges();
          
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
        public ActionResult CalculateSalary(DateTime Date, int[] usersId,int? a)
        {
           List<User> users = (List<User>)TempData["AllUsers"];

            if (usersId != null)
            {
                foreach (var id in usersId)
                {
                    if (CheckCalculatedUsers(Date.Month, Date.Year, id))
                        return Content("bu user artiq maasi hesblanmisdi");
                    User findedUser = db.Users.Find(id);
                    if (findedUser == null)
                        return Content("user empty");

                    //static keys
                    string bymonth = db.ButtonsStatics.FirstOrDefault(f => f.Key == "{ayliqgelir}").Key;
                    string byyear = db.ButtonsStatics.FirstOrDefault(f => f.Key == "{illikgelir}").Key;

                    string formula = findedUser.CalcForum.Formula;
                    if (ByMonth(id, Date.Month) == null)
                        return Content("bu iscinin secilmis ay uzre hec bir satisi yoxdu");
                    if(ByYear(id, Date.Year) == null)
                        return Content("bu iscinin secilmis illik uzre hec bir satisi yoxdu");
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
                            return Content("expression error");
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
                        return Content("catch");
                    }

                }

                return RedirectToAction("calculatedsalary");
            }
            return Content("500");

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