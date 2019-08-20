using DataAccessLayer;
using NCalc;
using SalaryCalc.Auth;
using SalaryCalc.Extensions;
using SalaryCalc.Filters;
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
            ViewBag.Users = db.Users.Where(w=>w.Postion.IsAdmin == false).ToList();
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

            Log log = new Log {

                CurrentUserId = userLoginned.Id,
                CreatedAt = DateTime.Now,
                UsedDate = forum.Date,
                Action = "CalcMethod",
                Controller = "Calculation"
            };
            db.Logs.Add(log);
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
            if (!ModelState.IsValid)
            {
                Session["Error"] = "Bütün xanaları doldurun";
                return RedirectToAction("index");
            }

            CalcForum calcForum = db.CalcForums.Find(forum.Id);
            calcForum.Name = forum.Name;
            calcForum.Formula = forum.Formula;

            Log log = new Log {
                CurrentUserId = userLoginned.Id,
                Controller ="Calculation",
                Action = "EditCalcMethod",
                CreatedAt = DateTime.Now,
                UsedDate = forum.Date
            };
            db.Logs.Add(log);
            LogCalcForum logCalcForum = new LogCalcForum {

                OldFormula = calcForum.Formula,
                OldName = calcForum.Name,
                Log =log
            };
                db.SaveChanges();
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
                Log log = new Log
                {
                    CurrentUserId = userLoginned.Id,
                    Controller = "Calculation",
                    Action = "DeleteCalcMethod",
                    CreatedAt = DateTime.Now,
                    UsedDate = calc.Date
                };
                db.Logs.Add(log);
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
        public ActionResult CalculateSalary(DateTime? Date, List<int> usersId)
        {
            if(Date == null)
            {
                Session["Error"] = "Tarix Secin";
                return RedirectToAction("calculatesalary");
            }
            bool? allChecked = (bool?)TempData["AllChecked"];
          
            List<int> allUsersId = new List<int>();
            List<User> users = (List<User>)TempData["AllUsers"];
            if (usersId == null && allUsersId.Count() == 0)
            {
                Session["Error"] = "Işçini Seçin!";
                return RedirectToAction("calculatesalary");
            }
                if (allChecked == true)
            {
                foreach (var item in users)
                {
                    allUsersId.Add(item.Id);
                }
            }
            Log log = new Log
            {
                Controller = "Calculation",
                Action = "CalculateSalary",
                CurrentUserId = userLoginned.Id,
                CreatedAt = DateTime.Now
            };
            db.Logs.Add(log);
            foreach (var id in allChecked == true ? allUsersId : usersId)
                    {
                        if (CheckCalculatedUsers(Date.Value.Month, Date.Value.Year, id))
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
                        string formula = findedUser.CalcForum.Formula.Trim();

                 if(!formula.Contains(bymonth) && !formula.Contains(byyear)
                    && !formula.Contains("{["+findedUser.PinCod+ "&ayliq]}") 
                    && !formula.Contains("{[" + findedUser.PinCod + "&illik]}"))
                {
                    Session["Error"] = "Method Duzgun Deyil!";
                    return RedirectToAction("calculatesalary");
                }
                        //({ayliqgelir} 
                        if (formula.Contains(bymonth))
                        {
                            if (ByMonth(id, Date.Value.Month) == null)
                            {
                                Session["Error"] = "İşçinin seçilmiş ay üzrə heç bir təsdiqlənmiş satışı yoxdu";
                                return RedirectToAction("calculatesalary");
                            }

                            string ByMonthValue = ByMonth(id, Date.Value.Month).ToString();
                            formula = formula.Replace(bymonth, ByMonthValue);
                        }
                        //{illikgelir}
                        if (formula.Contains(byyear))
                        {
                            if (ByYear(id, Date.Value.Year) == null)
                            {
                                Session["Error"] = "İşçinin seçilmiş il üzrə heç bir təsdiqlənmiş satışı yoxdu";
                                return RedirectToAction("calculatesalary");
                            }

                            string ByYearValue = ByYear(id, Date.Value.Year).ToString();
                            formula = formula.Replace(byyear, ByYearValue);
                        }

                        //=====================================
                        if (formula.Contains("$"))
                        {
                            if (formula.Matches("$") >= 3)
                            {
                                Session["Error"] = "Ancaq bir ədəd limit yaza bilərsiz!";
                                return RedirectToAction("calculatesalary");
                            }

                            List<string> limitedNumber = StringExtentions.EverythingBetween(formula, "$", "$");

                            string limitN = limitedNumber.FirstOrDefault().Split(',')[0];
                            string limitdate = limitedNumber.FirstOrDefault().Split(',')[1];
                            if (limitdate == "ayliq")
                            {
                                if (ByMonth(id, Date.Value.Month) == null)
                                {
                                    Session["Error"] = "İşçinin seçilmiş ay üzrə heç bir təsdiqlənmiş satışı yoxdu";
                                    return RedirectToAction("calculatesalary");
                                }
                                string ByMonthValue = ByMonth(id, Date.Value.Month).ToString();
                                if (Convert.ToDouble(ByMonthValue) > Convert.ToDouble(limitN))
                                {
                                    if (formula.Contains("[") && formula.Contains("]"))
                                    {
                                        //{[123456B&ayliq]} və ya {[234&illik]}
                                        List<string> userPinCodes = StringExtentions.EverythingBetween(formula, "[", "]");

                                        foreach (var item in userPinCodes)
                                        {
                                            string dateMonthOrYear = item.Split('&')[1];
                                            if (dateMonthOrYear == limitdate)
                                            {
                                                string pincode = item.Split('&')[0];
                                                string old = "{[" + item + "]}";
                                                User userCalcSalary = db.Users.FirstOrDefault(w => w.PinCod == pincode);
                                                if (userCalcSalary == null)
                                                {
                                                    Session["Error"] = "" + pincode + " Fin Kodlu işçi yoxdu";
                                                    return RedirectToAction("calculatesalary");
                                                }

                                                string symbol = "$" +limitedNumber.FirstOrDefault()+ "$";
                                                formula = formula.Replace(old, ByMonthValue).Replace(symbol,"");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Session["Error"] = "Qoyulmuş Limiti Kecmeyib";
                                    return RedirectToAction("calculatesalary");
                                }
                            }
                            if (limitdate == "illik")
                            {
                                if (ByYear(id, Date.Value.Year) == null)
                                {

                                    Session["Error"] = "İşçinin seçilmiş il üzrə heç bir təsdiqlənmiş satışı yoxdu";
                                    return RedirectToAction("calculatesalary");
                                }
                                string BPinCodeyYearValue = ByYear(id, Date.Value.Year).ToString();

                                if (Convert.ToDouble(BPinCodeyYearValue) > Convert.ToDouble(limitN))
                                {
                                    if (formula.Contains("[") && formula.Contains("]"))
                                    {
                                        //{[123456B&ayliq]} və ya {[234&illik]}
                                        List<string> userPinCodes1 = StringExtentions.EverythingBetween(formula, "[", "]");

                                        foreach (var item in userPinCodes1)
                                        {
                                            string dateMonthOrYear = item.Split('&')[1];
                                            if (dateMonthOrYear == limitdate)
                                            {
                                                string pincode = item.Split('&')[0];
                                                string old = "{[" + item + "]}";
                                                User userCalcSalary = db.Users.FirstOrDefault(w => w.PinCod == pincode);
                                                if (userCalcSalary == null)
                                                {
                                                    Session["Error"] = "" + pincode + " Fin Kodlu işçi yoxdu";
                                                    return RedirectToAction("calculatesalary");
                                                }

                                                string symbol = "$" + limitedNumber.FirstOrDefault() + "$";
                                                formula = formula.Replace(old, BPinCodeyYearValue).Replace(symbol, "");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Session["Error"] = "Qyulmuş Limiti Kecmeyib";
                                    return RedirectToAction("calculatesalary");
                                }
                            }
                            //
                        }
                        else
                        {
                            //{[123456B&ayliq]} və ya {[234&illik]}
                            List<string> userPinCodes2 = StringExtentions.EverythingBetween(formula, "[", "]");

                            foreach (var item in userPinCodes2)
                            {
                                string pincode = item.Split('&')[0];
                                string old = "{[" + item + "]}";
                                User userCalcSalary = db.Users.FirstOrDefault(w => w.PinCod == pincode);
                                if (userCalcSalary == null)
                                {
                                    Session["Error"] = "" + pincode + " Fin Kodlu işçi yoxdu";
                                    return RedirectToAction("calculatesalary");
                                }
                                string dateMonthOrYear = item.Split('&')[1];
                                if (dateMonthOrYear == "ayliq")
                                {
                                    if (ByMonth(userCalcSalary.Id, Date.Value.Month) == null)
                                    {
                                        Session["Error"] = "İşçinin seçilmiş ay üzrə heç bir təsdiqlənmiş satışı yoxdu";
                                        return RedirectToAction("calculatesalary");
                                    }

                                    string ByPinCodeMonthValue = ByMonth(id, Date.Value.Month).ToString();

                                    formula = formula.Replace(old, ByPinCodeMonthValue);
                                }
                                if (dateMonthOrYear == "illik")
                                {
                                    if (ByYear(userCalcSalary.Id, Date.Value.Year) == null)
                                    {

                                        Session["Error"] = "İşçinin seçilmiş il üzrə heç bir təsdiqlənmiş satışı yoxdu";
                                        return RedirectToAction("calculatesalary");
                                    }

                                    string BPinCodeyYearValue = ByYear(id, Date.Value.Year).ToString();

                                    formula = formula.Replace(old, BPinCodeyYearValue);
                                }
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
                                Date = (DateTime)Date
                            };

                            db.CalculatedSalaryByUsers.Add(calculated);

                    LogCalcSalary logCalc = new LogCalcSalary
                    {

                        OldSalary = (double)e.Evaluate(),
                        UserId = id,
                        Log = log,
                        Date = (DateTime)Date
                    };
                    db.LogCalcSalaries.Add(logCalc);


                }
                        catch (EvaluationException)
                        {
                            Session["Error"] = "Xəta!";
                            return RedirectToAction("calculatesalary");
                        }
                    }
            db.SaveChanges();

            return RedirectToAction("calculatedsalary");
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult AllChecked(bool? allChecked)
        {
            if (allChecked == null)
            {
                allChecked = false;
            }
            TempData["AllChecked"] = allChecked;
            return Json(allChecked, JsonRequestBehavior.AllowGet);
        }
        //Partial view
        [HttpGet]
        public PartialViewResult UsersForCalc(int? currMonth, int? currYear, int page = 1)
        {
            int skip = ((int)page - 1) * 6;

            List<User> model = db.Users.Where(w =>w.Postion.IsAdmin == false && 
            w.Sales.Any(a => a.Date.Year == currYear && a.Date.Month == currMonth)
            && w.CalculatedSalaryByUsers
            .Where(x => x.Date.Month == currMonth && x.Date.Year == currYear)
            .FirstOrDefault(a =>a.UserId == w.Id) == null).ToList();

            //Pagination List
            List<User> PaginateModel = db.Users.Where(w => w.Postion.IsAdmin == false
            &&
            w.Sales.Any(a => a.Date.Year == currYear && a.Date.Month == currMonth)
            && w.CalculatedSalaryByUsers
            .Where(x => x.Date.Month == currMonth && x.Date.Year == currYear)
            .FirstOrDefault(a => a.UserId == w.Id) == null).OrderBy(a => a.Id)
            .Skip(skip).Take(6).ToList().ToList();
            ViewBag.TotalPage = Math.Ceiling(model.Count() / 6.0);
            ViewBag.Page = page;
            ViewBag.CurrMonth = currMonth;
            ViewBag.CurrYear = currYear;
            TempData["AllUsers"] = model;

            if(TempData["AllChecked"] == null)
            {
                TempData["AllChecked"] = false;
            }
            ViewBag.AllChecked = (bool)TempData["AllChecked"];
            
            return PartialView("_UsersForCalcPartial", PaginateModel);
        }
      
        [NonAction]
        public double? ByMonth(int? id, int? currMonth)
        {

            if (db.Sales.FirstOrDefault(a => a.UserId == id && a.Date.Month == currMonth && a.IsComfirmed == true) == null)
                return null;
            double? total = db.Sales.Where(w => w.UserId == id && w.Date.Month == currMonth && w.IsComfirmed == true).Sum(s => s.Price);

            return total;
        }
        [NonAction]
        public double? ByYear(int? id, int? currYear)
        {
            if (db.Sales.FirstOrDefault(a => a.UserId == id && a.Date.Year == currYear && a.IsComfirmed == true) == null)
                return null;
            double? total = db.Sales.Where(w => w.UserId == id && w.Date.Year == currYear && w.IsComfirmed == true).Sum(s => s.Price);

            return total;
        }
        [NonAction]
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