using SalaryCalc.Auth;
using SalaryCalc.Dal;
using SalaryCalc.Filters;
using SalaryCalc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalaryCalc.Controllers
{
    [FilterContext]
    [RolesAuth]
    public class SalesController : BaseController
    {
        private readonly DataContext db = new DataContext();
        // GET: Sales
        public ActionResult Index()
        {
            return View(model: db.Sales.ToList());
        }
        //Get [baseUrl]Sales/Create
        [HttpGet]
        public ActionResult Create()
        {

            return View(model:db.Users.ToList());
        }
        //Post [baseUrl]Sales/Create
        [HttpPost]
        public ActionResult Create(Sale sale)
        {
            if (sale == null)
                return RedirectToAction("index");

            db.Sales.Add(sale);
            db.SaveChanges();

            return RedirectToAction("index");
        }
        //Get [baseUrl]Sales/Edit
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();

            ViewBag.User = db.Users.ToList();

            return View(model:db.Sales.Find(id));
        }
        //Post [baseUrl]Sales/Edit
        [HttpPost]
        public ActionResult Edit(Sale sale)
        {
            if(sale != null){
                db.Entry(sale).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("index");
            }
            return RedirectToAction("index");
        }
        //Get [baseUrl]Sales/Delete
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return HttpNotFound();

            Sale sale = db.Sales.Find(id);
            if(sale != null)
            {
                db.Sales.Remove(sale);
            }
            db.SaveChanges();

            return RedirectToAction("index");
        }
    }
}