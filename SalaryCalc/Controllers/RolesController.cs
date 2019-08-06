using SalaryCalc.Dal;
using SalaryCalc.Dtos;
using SalaryCalc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalaryCalc.Controllers
{
    public class RolesController : Controller
    {
        private readonly DataContext db = new DataContext();
        // GET: Roles
        public ActionResult Index()
        {
            return View();
        }
        //Get [baseUrl]Sales/Create
        [HttpGet]
        public ActionResult Create()
        {
            List<Role> roles = db.Roles.ToList();
            ViewBag.Postion = db.Postions.Where(w=>w.Status == true).ToList();
            
            return View(roles);
        }
        [HttpPost]
        public ActionResult Create(RoleIdsDto RoleIdsDto)
        {
            if (RoleIdsDto.Ids == null)
            {
                return Json(new { status = 404 }, JsonRequestBehavior.AllowGet);
            }
            foreach (var id in RoleIdsDto.Ids)
            {
                UserRole userRole = new UserRole {

                    RoleId = id,
                    PostionId = RoleIdsDto.PostionId

                };
                db.UserRoles.Add(userRole);
                db.SaveChanges();
            }



            return Json(new
            {
                status = 200,
                redirectUrl = Url.Action("index", "roles"),
                isRedirect = true
            }, JsonRequestBehavior.AllowGet);
        }

    }
}