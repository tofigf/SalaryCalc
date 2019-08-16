﻿using NCalc;
using SalaryCalc.Auth;
using SalaryCalc.Dal;
using SalaryCalc.Extensions;
using SalaryCalc.Filters;
using SalaryCalc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace SalaryCalc.Controllers
{
    [FilterContext]
    [RolesAuth]
    public class CalculationController : BaseController
    {

        #region CalcMethod
        // GET: Calculation
        [HttpGet]
        public ViewResult Index(int page = 1)
        {
            int skip = ((int)page - 1) * 10;

            ViewBag.TotalPage = Math.Ceiling(db.SaleImports.Count() / 10.0);
            ViewBag.Page = page;

         List<CalcForum> calcForums = db.CalcForums.OrderByDescending(o => o.Id).OrderByDescending(a => a.Id)
                 .Skip(skip).Take(10).ToList();
            return View(calcForums);
        }
        //Get [baseUrl]Calculation/CalcMethod
        [HttpGet]
        public ViewResult CalcMethod()
        {
            ViewBag.Users = db.Users.ToList();
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
            if (db.CalcForums.FirstOrDefault(f => f.Id == id).Users.Count() > 0)
            {
                Session["Error"] = "İstifadə Olunmuş Düsturu Dəyişmək Olmaz!";
                return RedirectToAction("index");

            }

            CalcForum forum = db.CalcForums.Find(id);
            if (forum != null)
                return View(model: forum);

            return RedirectToAction("index");
        }
        //Post [baseUrl]Calculation/EditCalcMethod
        [HttpPost]
        public ActionResult EditCalcMethod(CalcForum forum)
        {
            if (forum != null)
            {
                db.Entry(forum).State = EntityState.Modified;
                db.Entry(forum).Property(x => x.Date).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("index");
            }
            Session["Error"] = "Bütün xanaları doldurun";
            return RedirectToAction("index");
        }
        //Get [baseUrl]Sales/Delete
        [HttpGet]
        public ActionResult DeleteCalcMethod(int? id)
        {
            if (id == null)
                return HttpNotFound();
            if (db.CalcForums.FirstOrDefault(f => f.Id == id).Users.Count() > 0)
            {
                Session["Error"] = "İstifadə Olunmuş Düsturu Silmək Olmaz!";
                return RedirectToAction("index");

            }

            CalcForum calc = db.CalcForums.Find(id);
            if (calc != null)
            {
                db.CalcForums.Remove(calc);
            }
            db.SaveChanges();
            Session["Success"] = "Müvəfəqiyyətlə Silindi!";
            return RedirectToAction("index");
        }
        #endregion

        #region CalculateSalary
        //Get [baseUrl]Calculation/CalculatedSalary
        [HttpGet]
        public ViewResult CalculatedSalary(int page = 1)
        {
            int skip = ((int)page - 1) * 10;

            ViewBag.TotalPage = Math.Ceiling(db.Sales.Count() / 10.0);
            ViewBag.Page = page;

            List<CalculatedSalaryByUser> salaryByUsers = db.CalculatedSalaryByUsers.OrderByDescending(a => a.Id)
                 .Skip(skip).Take(10).ToList();

            return View(salaryByUsers);
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
                            Session["Error"] = "İstifadəçinin Maaşı Hesablanmışdı";
                            return RedirectToAction("calculatesalary");
                        }

                        User findedUser = db.Users.Find(id);

                        if (findedUser == null)
                        {
                            Session["Error"] = "İşçi Yoxdu";
                            return RedirectToAction("calculatesalary");
                        }

                        //static keys
                        string bymonth = db.ButtonsStatics.FirstOrDefault(f => f.Key == "{ayliqgelir}").Key;
                        string byyear = db.ButtonsStatics.FirstOrDefault(f => f.Key == "{illikgelir}").Key;
                        //Find Each User Formula
                        string formula = findedUser.CalcForum.Formula;


                        if (formula.Contains(bymonth))
                        {
                            if (ByMonth(id, Date.Month) == null)
                            {
                                Session["Error"] = "İşçinin seçilmiş ay üzrə heç bir təsdiqlənmiş satışı yoxdu";
                                return RedirectToAction("calculatesalary");
                            }

                            string ByMonthValue = ByMonth(id, Date.Month).ToString();
                            formula = formula.Replace(bymonth, ByMonthValue);
                        }
                        if (formula.Contains(byyear))
                        {
                            if (ByYear(id, Date.Year) == null)
                            {
                                Session["Error"] = "İşçinin seçilmiş il üzrə heç bir təsdiqlənmiş satışı yoxdu";
                                return RedirectToAction("calculatesalary");
                            }

                            string ByYearValue = ByYear(id, Date.Year).ToString();
                            formula = formula.Replace(byyear, ByYearValue);
                        }

                        List<string> userPinCodes = StringExtentions.EverythingBetween(formula, "[", "]");

                            foreach (var item in userPinCodes)
                            {
                                string pincode = item.Split('&')[0];
                                string old = "{[" + item + "]}";
                                User userCalcSalary = db.Users.FirstOrDefault(w => w.PinCod == pincode);
                                if(userCalcSalary == null)
                                {
                                    Session["Error"] = ""+ pincode + " Fin Kodlu işçi yoxdu";
                                    return RedirectToAction("calculatesalary");
                                }
                                string dateMonthOrYear = item.Split('&')[1];
                                if (dateMonthOrYear == "ayliq")
                                {
                                    if (ByMonth(userCalcSalary.Id, Date.Month) == null)
                                    {
                                        Session["Error"] = "İşçinin seçilmiş ay üzrə heç bir təsdiqlənmiş satışı yoxdu";
                                        return RedirectToAction("calculatesalary");
                                    }

                                    string ByPinCodeMonthValue = ByMonth(id, Date.Month).ToString();

                                    formula = formula.Replace(old, ByPinCodeMonthValue);
                                }
                                 if (dateMonthOrYear == "illik")
                                {
                                    if (ByYear(userCalcSalary.Id, Date.Year) == null)
                                    {
                                       

                                        Session["Error"] = "İşçinin seçilmiş il üzrə heç bir təsdiqlənmiş satışı yoxdu";
                                        return RedirectToAction("calculatesalary");
                                    }

                                    string BPinCodeyYearValue = ByYear(id, Date.Year).ToString();

                                    formula = formula.Replace(old, BPinCodeyYearValue);
                                }
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
                            Session["Error"] = "Xəta!";
                            return RedirectToAction("calculatesalary");
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
                            Session["Error"] = "İşçinin seçilmiş ay üzrə heç bir satışı təsdiqlənmiş yoxdu";
                            return RedirectToAction("calculatesalary");
                        }
                        if (ByYear(userr.Id, Date.Year) == null)
                        {
                            Session["Error"] = "İşçinin seçilmiş il üzrə heç bir satışı təsdiqlənmiş yoxdu";
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
                            Session["Error"] = "Xəta!";
                            return RedirectToAction("calculatesalary");
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
        public PartialViewResult UsersForCalc(int? currMonth, int? currYear, int page = 1)
        {
            int skip = ((int)page - 1) * 6;

            List<User> model = db.Users.Where(w => w.CalculatedSalaryByUsers
            .Where(x => x.Date.Month == currMonth && x.Date.Year == currYear)
            .FirstOrDefault(a => a.UserId == w.Id) == null).ToList();

            //Pagination list
            List<User> PaginateModel = db.Users.Where(w => w.CalculatedSalaryByUsers
            .Where(x => x.Date.Month == currMonth && x.Date.Year == currYear)
            .FirstOrDefault(a => a.UserId == w.Id) == null).OrderBy(a => a.Id)
            .Skip(skip).Take(6).ToList().ToList();
            ViewBag.TotalPage = Math.Ceiling(model.Count() / 6.0);
            ViewBag.Page = page;
            ViewBag.CurrMonth = currMonth;
            ViewBag.CurrYear = currYear;
            TempData["AllUsers"] = model;

            return PartialView("_UsersForCalcPartial", PaginateModel);
        }

        public double? ByMonth(int? id, int? currMonth)
        {

            if (db.Sales.FirstOrDefault(a => a.UserId == id && a.Date.Month == currMonth && a.IsComfirmed == true) == null)
                return null;
            double? total = db.Sales.Where(w => w.UserId == id && w.Date.Month == currMonth && w.IsComfirmed == true).Sum(s => s.Price);

            return total;
        }
        public double? ByYear(int? id, int? currYear)
        {
            if (db.Sales.FirstOrDefault(a => a.UserId == id && a.Date.Year == currYear && a.IsComfirmed == true) == null)
                return null;
            double? total = db.Sales.Where(w => w.UserId == id && w.Date.Year == currYear && w.IsComfirmed == true).Sum(s => s.Price);

            return total;
        }
        public bool CheckCalculatedUsers(int? currMonth, int? currYear, int? userId)
        {
            if (db.CalculatedSalaryByUsers.FirstOrDefault(w => w.UserId == userId && w.Date.Month == currMonth && w.Date.Year == currYear) != null)
            {
                return true;
            }
            return false;
        }
        #endregion
      
    }
}