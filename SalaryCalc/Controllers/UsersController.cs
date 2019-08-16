using SalaryCalc.Auth;
using SalaryCalc.Dal;
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

         List<User> users =  db.Users.Where(w => w.IsAdmin != 0 && w.IsAdmin != 1).OrderByDescending(a => a.Id)
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
            db.SaveChanges();

            return RedirectToAction("index");
        }
        //Get [baseUrl]Users/Edit
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!ModelState.IsValid)
            {
                Session["Error"] = "Bütün xanaları doldurun";
                return RedirectToAction("index");
            }
            if (id == null)
                return HttpNotFound();

            User user = db.Users.Find(id);
            ViewBag.CalcForum = db.CalcForums.ToList();
            ViewBag.Postion = db.Postions.Where(w => w.Status == true).ToList();
            return View(user);
        }
        //Post [baseUrl]Users/Edit
        [HttpPost]
        public ActionResult Edit(User user,string OldPassword)
        {
           
            var us = db.Users.Find(user.Id);
            if (!Crypto.VerifyHashedPassword(us.Password, OldPassword))

                if (!ModelState.IsValid)
                {
                    Session["Error"] = "Köhnə Şifrə Səhvdi";
                    return RedirectToAction("index");
                }

               if (!ModelState.IsValid)
            {
                Session["Error"] = "Xanaları Düzgün Doldurun";
                return RedirectToAction("index");
            }


            us.UserName = user.UserName;
            us.FullName = user.FullName;
            us.PostionId = user.PostionId;
            us.Email = user.Email;
            us.CalcForumId = user.CalcForumId;
            us.Password = Crypto.HashPassword(user.Password);

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
       [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckUsersPassword(string OldPassword)
        {
            int Id = (int)Session["UserId"];

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