using SalaryCalc.Dal;
using SalaryCalc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SalaryCalc.Controllers
{
    public class LoginController : Controller
    {
        private readonly DataContext db = new DataContext();
        // GET: Login
        public ActionResult Index()
        {

            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(User user)
        {
            if (user == null)
                //return RedirectToAction("index");
                return Content("null");
            if (user.Email == null || user.Password == null)
            {
                Session["LoginError"] = "Boşluq buraxmayın";
                return Content("bosluq");
            }
            User loginned = db.Users.FirstOrDefault(u => u.Email.ToLower() == user.Email.ToLower());

            if (loginned != null)
            {
                if (Crypto.VerifyHashedPassword(loginned.Password, user.Password))
                {

                    Session["LoggedUser"] = loginned;
                    Session["UserId"] = loginned.Id;

                    return RedirectToAction("index", "home");
                }
            }
            Session["LoginError"] = "E-poçt və ya şifrə səhvdir";

            return RedirectToAction("index");
        }

        public ActionResult Logout()
        {
            Session["LoggedUser"] = null;
            Session["UserId"] = null;
            return RedirectToAction("index");
        }

    }
}