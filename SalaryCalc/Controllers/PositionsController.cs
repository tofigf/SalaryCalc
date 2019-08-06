﻿using SalaryCalc.Dal;
using SalaryCalc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalaryCalc.Controllers
{
    public class PositionsController : Controller
    {
        private readonly DataContext db = new DataContext();
        // GET: Positions
        public ActionResult Index()
        {
            
            return View(model:db.Postions.Where(s=>s.Status == true).ToList());
        }
        //Post [baseUrl]Positions/Create
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(Postion postion)
        {
            if (!ModelState.IsValid)
                return Content("required");
            if (postion == null)
                return RedirectToAction("index");

            postion.Status = true;
            db.Postions.Add(postion);
            db.SaveChanges();

            return RedirectToAction("index");
        }
        //Get [baseUrl]Positions/Edit
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Postion postion = db.Postions.Find(id);


            return View(postion);

        }
        //Get [baseUrl]Positions/Edit
        [HttpPost]
        public ActionResult Edit(Postion postion)
        {
            db.Entry(postion).State = EntityState.Modified;
            db.Entry(postion).Property(o => o.Status).IsModified = false;

            db.SaveChanges();

            return RedirectToAction("index");
        }
        //Get [baseUrl]Positions/Delete
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Postion postion = db.Postions.Find(id);

            postion.Status = false;
            db.SaveChanges();

            return RedirectToAction("index");
        }
    }
}