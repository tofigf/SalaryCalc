using DataAccessLayer;
using SalaryCalc.Auth;
using SalaryCalc.Filters;
using SalaryCalc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SalaryCalc.Controllers
{
    [FilterContext]
    [RolesAuth]
    public class UsersController : BaseController
    {
        // GET: Users
        public ActionResult Index(int page = 1)
        {

            int skip = ((int)page - 1) * 10;

            ViewBag.TotalPage = Math.Ceiling(db.SaleImports.Count() / 10.0);
            ViewBag.Page = page;

         List<User> users =  db.Users.Where(w => w.Postion.IsAdmin == false).OrderByDescending(a => a.Id)
                 .Skip(skip).Take(10).ToList();
            return View(users);
        }
        //Get [baseUrl]Users/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.CalcForum = db.CalcForums.ToList();
            return View(model:db.Postions.Where(w=>w.Status == true).ToList());
        }
        //Post [baseUrl]Users/Create
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(User user)
        {
            if (!ModelState.IsValid)
            {
                Session["Error"] = "Xanaları Düzgün Doldurun";
                return RedirectToAction("index");
            }
   
            if (user == null)
            {
                Session["Error"] = "Bütün xanaları doldurun";
                return RedirectToAction("index");
            }
            user.Password = Crypto.HashPassword(user.Password);
            db.Users.Add(user);

            //Log
            Log log = new Log {
                CurrentUserId = userLoginned.Id,
                ActionedUser = user,
                CreatedAt  =DateTime.Now,
                Action = "Create",
                Controller = "Users"
            };
            db.Logs.Add(log);
            db.SaveChanges();


            return RedirectToAction("index");
        }
        //Get [baseUrl]Users/Edit
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();

            User user = db.Users.Find(id);
            Session["EditUserId"] = user.Id;
            ViewBag.CalcForum = db.CalcForums.ToList();
            ViewBag.Postion = db.Postions.Where(w => w.Status == true).ToList();
            return View(user);
        }
        //Post [baseUrl]Users/Edit
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(User user, string OldPassword)
        {
            User us = db.Users.Find(user.Id);
            if (us == null)
            {
                Session["Error"] = "Xəta!";
                return RedirectToAction("index");
            }

            if (!ModelState.IsValid)
            {
                Session["Error"] = "Xəta!";
                return RedirectToAction("index");
            }
            //Log
            Log log = new Log
            {
                CurrentUserId = userLoginned.Id,
                ActionedUserId = us.Id,
                CreatedAt = DateTime.Now,
                Action = "Edit",
                Controller = "Users"
            };
            db.Logs.Add(log);
            LogUser logUser = new LogUser
            {
                OldUserName = us.UserName,
                OldFullName = us.FullName,
                OldEmail = us.Email,
                OldPhone = us.Phone,
                OldPinCod = us.PinCod,
                Log = log
            };
            db.LogUsers.Add(logUser);

            us.UserName = user.UserName;
            us.FullName = user.FullName;
            us.PostionId = user.PostionId;
            us.Phone = user.Phone;
            us.Email = user.Email;
            us.CalcForumId = user.CalcForumId;

            db.SaveChanges();
            return RedirectToAction("index");
        }
        //Get [baseUrl]Users/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return HttpNotFound();

            User user = db.Users.Find(id);
            if(user == null)
            {
                Session["Error"] = "İşçi yoxdu";
                return RedirectToAction("index");
            }
            if(user != null)
            {
                db.Users.Remove(user);
            }
            db.SaveChanges();

            return RedirectToAction("index");
        }
        [HttpGet]
        public ActionResult PasswordChange(int? id)
        {
            if (id == null)
                return HttpNotFound();

            User user = db.Users.Find(id);
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult PasswordChange(User user ,string OldPassword)
        {
            User findedUser = db.Users.Find(user.Id);
            if (!Crypto.VerifyHashedPassword(findedUser.Password, OldPassword))
            {
                findedUser.Password = Crypto.HashPassword(user.Password);
                db.SaveChanges();
            }
            return RedirectToAction("index");
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckUsersPassword(string OldPassword)
        {
            int Id = (int)Session["EditUserId"];

            User loginned = db.Users.Find(Id);

            if (loginned == null)
            {
                return Json(new
                {
                    valid = false,
                    message = ""
                }, JsonRequestBehavior.AllowGet);

            }
                if (!Crypto.VerifyHashedPassword(loginned.Password, OldPassword))
                    {
                    return Json(new
                    {
                        valid = false,
                        message = "Şifrə yanlışdı"
                    }, JsonRequestBehavior.AllowGet);

                }
               
                    return Json(new
                    {
                        valid = true
                    }, JsonRequestBehavior.AllowGet);
                

           
        }

    }
}