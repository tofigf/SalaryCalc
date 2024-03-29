﻿using DataAccessLayer;
using SalaryCalc.Auth;
using SalaryCalc.Filters;
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
        // GET: Roles
        public ActionResult Index()
        {
            Models.VwModel.VwRoles vw = new Models.VwModel.VwRoles
            {
                Roles = db.Roles.ToList(),
                UserRoles = db.UserRoles.ToList(),
                Postions = db.Postions.Where(w => w.Status == true).ToList()
            };

            return View(vw);
        }

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
                        newRole.RoleId = (int)role;
                        newRole.Date = DateTime.Now.Date;
                        db.UserRoles.Add(newRole);
                    }
                }
                db.SaveChanges();
               
            }
            return RedirectToAction("index","roles");
        }


    }
}