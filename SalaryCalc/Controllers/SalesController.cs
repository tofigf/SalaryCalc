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
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();

            ViewBag.User = db.Users.ToList();

            return View(model:db.Sales.Find(id));
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
            List<SalesGroupFileDto> groupFileDtos = new List<SalesGroupFileDto>();
            var sales = db.Sales
                .Where(w => w.FileUrl != null)
                .OrderByDescending(d =>d.Id)
                .GroupBy(g => g.UserId)
                .Select(s => new {

                userId = s.Key,
                userName = s.FirstOrDefault().User.UserName,
                userFullname =s.FirstOrDefault().User.FullName,
                userEmail =s.FirstOrDefault().User.Email,
                userPhone =s.FirstOrDefault().User.Phone,
                productName =s.FirstOrDefault().Name,
                totalPrice = s.Sum(su => su.Price),
                totalCount = s.Sum(so=>so.Count),

            }).ToList();
            foreach (var item in sales)
            {
                SalesGroupFileDto groupFileDto = new SalesGroupFileDto {
                     UserId =item.userId,
                     UserName =item.userName,
                     UserFullname =item.userFullname,
                     UserEmail = item.userEmail,
                     UserPhone =item.userPhone,
                     ProductName = item.productName,
                     TotalPrice = item.totalPrice,
                     TotalCount =item.totalCount
                };
                groupFileDtos.Add(groupFileDto);
            }
        
            return View(groupFileDtos);
        }
        [AllowAnonymous]
        public FileResult DownloadSample()
        {
            string path = "~/Files/Sample.xlsx";
            return File(path, "application/vnd.ms-excel", "Nümunə.xlsx");
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
        public ActionResult Import(int? UserId, HttpPostedFileBase File)
        {
            if(UserId == null)
            {
                Session["uploadError"] = "İşçini Seçin";
                return RedirectToAction("getimport");
            }
            if(File == null)
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
            string filename = Guid.NewGuid().ToString() + File.FileName;
            string path = Path.Combine(Server.MapPath("~/Uploads"), filename);
            File.SaveAs(path);

            XLWorkbook xLWorkbook = new XLWorkbook(path);
            XLWorkbook workbook = xLWorkbook;
            var rows = workbook.Worksheet(1).RangeUsed().RowsUsed().Skip(1);

            foreach (var row in rows)
            {
             
                int.TryParse(row.Cell(5).Value.ToString(), out int vip);
                int.TryParse(row.Cell(6).Value.ToString(), out int discount);
                int.TryParse(row.Cell(3).Value.ToString(), out int count);
                double.TryParse(row.Cell(2).Value.ToString(), out double price);
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
                    if (count == 0)
                    {
                        Session["uploadError"] = "Məhsulun Sayı Boşdu";
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads"), filename)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Uploads"), filename));
                        }
                        return RedirectToAction("getimport");
                    }
                    if (string.IsNullOrEmpty(row.Cell(3).Value.ToString()))
                    {
                        Session["uploadError"] = "Məhsulun Tarix Boşdu";
                        if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Uploads"), filename)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Uploads"), filename));
                        }
                        return RedirectToAction("getimport");
                    }
                    if (!Regex.IsMatch(row.Cell(2).Value.ToString(), @"^-?[0-9][0-9,\.]+$"))
                    {
                        Session["uploadError"] = "Məhsulun Qiyməti Rəqəm Olmaldı";
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

                    Sale sale = new Sale
                    {
                        Name = row.Cell(1).Value.ToString(),
                        Price = price,
                        Count = count,
                        Date = Convert.ToDateTime(row.Cell(4).Value.ToString()),
                        Vip = vip == 1 ? true : false,
                        DisCount = discount == 1 ? true : false,
                        FileUrl = filename,
                        UserId = Convert.ToInt32(UserId)
                    };
                    db.Sales.Add(sale);

                    db.SaveChanges();
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
            Session["uploadSucces"] = "Müvəfəqiyyətlə Yükləndi";
            return RedirectToAction("getimport");

        }
    }
}