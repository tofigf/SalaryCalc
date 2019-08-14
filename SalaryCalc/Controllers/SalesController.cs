using ClosedXML.Excel;
using SalaryCalc.Auth;
using SalaryCalc.Dal;
using SalaryCalc.Dtos;
using SalaryCalc.Filters;
using SalaryCalc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        public ActionResult Index(int page = 1)
        {
            int skip = ((int)page - 1) * 10;

            List<Sale> model = db.Sales.OrderByDescending(a => a.Id)
                 .Skip(skip).Take(10).ToList();


            ViewBag.TotalPage = Math.Ceiling(db.Sales.Count() / 10.0);
            ViewBag.Page = page;

            return View(model);
        }
        //Get [baseUrl]Sales/Create
        [HttpGet]
        public ActionResult Create()
        {

            return View(model:db.Users.ToList());
        }
        //Post [baseUrl]Sales/Create
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(Sale sale)
        {
            if (sale == null)
            {
                Session["Error"] = "Bütün xanaları doldurun";
                return RedirectToAction("index");
            }

            db.Sales.Add(sale);
            db.SaveChanges();

            return RedirectToAction("index");
        }
        //Get [baseUrl]Sales/Edit
        [FilterConfirm]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();

            Sale model = db.Sales.Find(id);


            ViewBag.User = db.Users.ToList();

            return View(model);
        }
        //Post [baseUrl]Sales/Edit
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(Sale sale)
        {
            if(sale != null){
                db.Entry(sale).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("index");
            }
          
                Session["Error"] = "Bütün xanaları doldurun";
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
        //Get [baseUrl]Sales/GetImport
        [HttpGet]
        public ActionResult GetImport()
        {
        
            return View(model:db.SaleImports.ToList());
        }
        [AllowAnonymous]
        public FileResult DownloadSample()
        {
            string path = "~/Files/Sample.xlsx";
            return File(path, "application/vnd.ms-excel", "Nümunə.xlsx");
        }
        [AllowAnonymous]
        public FileResult DownloadImportedExcel(int? id)
        {
            if (id == null)
                return null;

            SaleImport saleImport = db.SaleImports.Find(id);
            if (saleImport == null)
                return null;

            string path = Server.MapPath("~/Uploads/" + saleImport.FileUrl);

            return File(path, "application/vnd.ms-excel", saleImport.Name);
        }
        //DeleteImported excel and data
        public ActionResult DeleteImported(int? id)
        {
            SaleImport saleImport = db.SaleImports.Find(id);
            List<Sale> sales = db.Sales.Where(w => w.SaleImportId == saleImport.Id).ToList();
             if(sales.Any(a=>a.IsComfirmed ==true))
                {

                Session["uploadError"] = "Yüklənmiş excel faylının  satışlarından hər hansı biri təsdiqlənmişdi(yalnız tək tək silmək olar)";
                return RedirectToAction("getimport");
            }
           
            if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads"), saleImport.FileUrl)))
            {
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/Uploads"), saleImport.FileUrl));
            }
            db.Sales.RemoveRange(sales);
            db.SaleImports.Remove(saleImport);
            db.SaveChanges();
            return RedirectToAction("getimport");
        }
        //Get [baseUrl]Sales/Import
        [HttpGet]
        public ActionResult Import()
        {
    
            return View(model:db.Users.ToList());
        }
        //Post [baseUrl]Sales/Import
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Import( HttpPostedFileBase File)
        {
            //Check
            #region Check
            if (File == null)
            {
                Session["uploadError"] = "Fayl Seçin";
                return RedirectToAction("getimport");
            }
            if (File.ContentLength > 1048576)
            {
                Session["uploadError"] = "Bu Fayl 1 mg yuxarıdır.";
                return RedirectToAction("getimport");
            }
            if (File.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" && File.ContentType != "application/vnd.ms-excel")
            {
                Session["uploadError"] = "Bu fayl tipi qebul deyil!";
                return RedirectToAction("getimport");
            }
            #endregion

            string filename = Guid.NewGuid().ToString() + File.FileName;
            string path = Path.Combine(Server.MapPath("~/Uploads"), filename);
            File.SaveAs(path);

            //SaleImport
            SaleImport saleImport = new SaleImport {
                Date = DateTime.Now,
                FileUrl = filename,
                Name = File.FileName
            };

            XLWorkbook xLWorkbook = new XLWorkbook(path);
            XLWorkbook workbook = xLWorkbook;
            var rows = workbook.Worksheet(1).RangeUsed().RowsUsed().Skip(1);

            foreach (var row in rows)
            {
             
                int.TryParse(row.Cell(6).Value.ToString(), out int vip);
                int.TryParse(row.Cell(7).Value.ToString(), out int discount);
                int.TryParse(row.Cell(4).Value.ToString(), out int count);
                double.TryParse(row.Cell(3).Value.ToString(), out double price);
                if (row.Cell(1).Value.ToString().Length > 0)
                {
                    //Check
                    #region Ckeck
                    if (price == 0)
                    {
                        Session["uploadError"] = "Məhsulun Qiyməti Boşdu";
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads"), filename)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Uploads"), filename));
                        }
                        return RedirectToAction("getimport");
                    }
                    if (string.IsNullOrEmpty(row.Cell(2).Value.ToString()))
                    {
                        Session["uploadError"] = "PinKod Boşdu";
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads"), filename)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Uploads"), filename));
                        }
                        return RedirectToAction("getimport");
                    }
                    if (count == 0)
                    {
                        Session["uploadError"] = "Məhsulun Sayı Boşdu";
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads"), filename)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Uploads"), filename));
                        }
                        return RedirectToAction("getimport");
                    }
                    if (string.IsNullOrEmpty(row.Cell(5).Value.ToString()))
                    {
                        Session["uploadError"] = "Məhsulun Tarix Boşdu";
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads"), filename)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Uploads"), filename));
                        }
                        return RedirectToAction("getimport");
                    }
                    if (!Regex.IsMatch(row.Cell(3).Value.ToString(), @"^-?[0-9][0-9,\.]+$"))
                    {
                        Session["uploadError"] = "Məhsulun Qiyməti Rəqəm Olmaldı";
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads"), filename)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Uploads"), filename));
                        }
                        return RedirectToAction("getimport");
                    }
                    if(!Regex.IsMatch(row.Cell(2).Value.ToString(), @"^[A-Z0-9]+$"))
                    {
                        Session["uploadError"] = "Pin Kod Rəqəm və Hərf Olmalıdı";
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads"), filename)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Uploads"), filename));
                        }
                        return RedirectToAction("getimport");
                    }
                    string subName = row.Cell(1).Value.ToString().Substring(0, 2);
                    if (!Regex.IsMatch(subName, @"^[a-zA-Z]{1,25}$"))
                    {
                        Session["uploadError"] = "Məhsulun Adı Hərflə Başlamalıdı";
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads"), filename)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Uploads"), filename));
                        }
                        return RedirectToAction("getimport");
                    }
                    #endregion

                    var pincodFromExcel = row.Cell(2).Value.ToString();
                    User user = db.Users.FirstOrDefault(f => f.PinCod == pincodFromExcel);

                    //Check
                    #region Check
                    if (user == null)
                    {
                        Session["uploadError"] = " Pin Kod Düzgün Deyil!";
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads"), filename)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Uploads"), filename));
                        }
                        return RedirectToAction("getimport");
                    }
                    DateTime.TryParse(row.Cell(5).Value.ToString(), out DateTime date);
                    if (CheckCalculatedUsers(date.Month, date.Year, user.Id))
                    {
                        var culture = new System.Globalization.CultureInfo("az");

                        Session["uploadError"] = "" + user.UserName + " Maaşı " + date.ToString("MMMM yyyy", culture) + " Tarixində Hesablanmışdı";
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads"), filename)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Uploads"), filename));
                        }
                        return RedirectToAction("getimport");
                    }
                    #endregion

                    Sale sale = new Sale
                    {
                        Name = row.Cell(1).Value.ToString(),
                        Price = price,
                        Count = count,
                        Date = Convert.ToDateTime(row.Cell(5).Value.ToString()),
                        Vip = vip == 1 ? true : false,
                        DisCount = discount == 1 ? true : false,
                        SaleImport = saleImport,
                        UserId = user.Id,
                        IsImported = true,
                        IsComfirmed = false
                    };
                    db.Sales.Add(sale);
                }
                else
                {
                    Session["uploadError"] = "Excel Faylı boşdu";
                    if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads"), filename)))
                    {
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Uploads"), filename));
                    }
                    return RedirectToAction("getimport");
                }
            }
            db.SaveChanges();
            Session["uploadSucces"] = "Müvəfəqiyyətlə Yükləndi";
            return RedirectToAction("getimport");
        }
        [HttpGet]
        public ActionResult ImporteData(int? id)
        {
            if (id == null)
                return HttpNotFound();
            List<Sale> sales = db.Sales.Where(w => w.SaleImportId == id).ToList();

            return View(sales);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ConfirmSales(int[] ids)
        {
            if (ids == null)
            {

                return Json(new { status = 409 }, JsonRequestBehavior.AllowGet);
            }
            
            foreach (var id in ids)
            {
                Sale sale = db.Sales.Find(id);
                if (sale != null)
                {
                    sale.IsComfirmed = true;
                    db.SaveChanges();
                }
            }
            return Json(new
            {
                status = 200,
                redirectUrl = Url.Action("index"),
                isRedirect = true
            }, JsonRequestBehavior.AllowGet);
        }

        public bool CheckCalculatedUsers(int? currMonth, int? currYear, int? userId)
        {
            if (db.CalculatedSalaryByUsers.FirstOrDefault(w => w.UserId == userId && w.Date.Month == currMonth && w.Date.Year == currYear) != null)
            {
                return true;
            }
            return false;
        }
    }
}