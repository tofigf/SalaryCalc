using SalaryCalc.Auth;
using SalaryCalc.Dal;
using SalaryCalc.Dtos;
using SalaryCalc.Filters;
using SalaryCalc.Models;
using SalaryCalc.Models.VwModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalaryCalc.Controllers
{
    [FilterContext]
    [RolesAuth]
    public class RolesController : BaseController
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
            VwRoles vw = new VwRoles {
                 Roles = db.Roles.ToList(),
                 UserRoles = db.UserRoles.ToList(),
                Postions = db.Postions.Where(w=>w.Status == true).ToList() 
            };
            //List<Role> roles = db.Roles.ToList();
            //ViewBag.Postion = db.Postions.Where(w=>w.Status == true).ToList();
            
            return View(vw);
        }
        //[HttpPost]
        //public ActionResult Create(RoleIdsDto RoleIdsDto)
        //{
        //    if (RoleIdsDto.Ids == null)
        //    {
        //        return Json(new { status = 404 }, JsonRequestBehavior.AllowGet);
        //    }
        //    foreach (var id in RoleIdsDto.Ids)
        //    {
        //        UserRole userRole = new UserRole {

        //            RoleId = id,
        //            PostionId = RoleIdsDto.PostionId

        //        };
        //        db.UserRoles.Add(userRole);
        //        db.SaveChanges();
        //    }



        //    return Json(new
        //    {
        //        status = 200,
        //        redirectUrl = Url.Action("index", "roles"),
        //        isRedirect = true
        //    }, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult Create(int id, int?[] Roles)
        {

            if (db.Postions.FirstOrDefault(f => f.Id == id) != null)
            {
                List<UserRole> userRoles = db.UserRoles.Where(w => w.PostionId == id).ToList();
                db.UserRoles.RemoveRange(userRoles);
                if (Roles != null)
                {
                    foreach (var role in Roles)
                    {
                        UserRole newRole = new UserRole();
                        newRole.PostionId = id;
                        newRole.RoleId = role ?? 0;
                        newRole.Date = DateTime.Now.Date;
                        db.UserRoles.Add(newRole);

                    }
                }
                db.SaveChanges();
            }
            return RedirectToAction("index");
        }


    }
}