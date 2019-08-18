using ClosedXML.Excel;
using SalaryCalc.Auth;
using SalaryCalc.Dtos;
using SalaryCalc.Filters;
using SalaryCalc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalaryCalc.Controllers
{
    [FilterContext]
    [RolesAuth]
    public class ReportsController : BaseController
    {

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        #region Salary
        [HttpGet]
        public ActionResult SalaryReportByMonth(int? year)
        {
            if(year == null)
            {
                year = DateTime.Now.Year;
            }
            List<SalaryReportByDateDto> monthDtos = new List<SalaryReportByDateDto>();
            var sqlMinDate = (DateTime)SqlDateTime.MinValue;

            var SalaryByMonyh = db.CalculatedSalaryByUsers.Where(w=>w.Date.Year == year).OrderBy(o => o.Date.Month)
                .GroupBy(o => SqlFunctions.DateAdd("month", SqlFunctions.DateDiff("month", sqlMinDate, o.Date), sqlMinDate))
                .Select(s => new {

                    date = s.Key,
                    total = s.Sum(su=>su.Salary)

                }).ToList();

            foreach (var item in SalaryByMonyh)
            {
                SalaryReportByDateDto monthDto = new SalaryReportByDateDto {

                    Date = item.date,
                    Total = item.total

                };
                monthDtos.Add(monthDto);
            }
            return View(monthDtos);
        }
        [HttpGet]
        public ActionResult SalaryReportByYear(int? year)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            List<SalaryReportByDateDto> monthDtos = new List<SalaryReportByDateDto>();
            var sqlMinDate = (DateTime)SqlDateTime.MinValue;

            var SalaryByMonyh = db.CalculatedSalaryByUsers.Where(w => w.Date.Year == year).OrderBy(o => o.Date.Month)
                .GroupBy(o => SqlFunctions.DateAdd("year", SqlFunctions.DateDiff("year", sqlMinDate, o.Date), sqlMinDate))
                .Select(s => new {

                    date = s.Key,
                    total = s.Sum(su => su.Salary)

                }).ToList();

            foreach (var item in SalaryByMonyh)
            {
                SalaryReportByDateDto monthDto = new SalaryReportByDateDto
                {

                    Date = item.date,
                    Total = item.total

                };
                monthDtos.Add(monthDto);
            }
            return View(monthDtos);
        }
        [HttpGet]
        public ActionResult SalaryReportByQuarterly(int? year)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            List<SalaryReportByDateDto> monthDtos = new List<SalaryReportByDateDto>();
            var sqlMinDate = (DateTime)SqlDateTime.MinValue;

            var SalaryByMonyh = db.CalculatedSalaryByUsers.Where(w => w.Date.Year == year).OrderBy(o => o.Date.Month)
                .GroupBy(o => (o.Date.Month - 1) / 3 + 1)
                .Select(s => new {

                    quarterly = s.Key,

                    total = s.Sum(su => su.Salary)

                }).ToList();

            foreach (var item in SalaryByMonyh)
            {
                SalaryReportByDateDto monthDto = new SalaryReportByDateDto
                {

                    Quarterly = (byte)item.quarterly,
                    Total = item.total

                };
                monthDtos.Add(monthDto);
            }
            return View(monthDtos);
        }
        [HttpGet]
        public ActionResult SalaryReportByWorkers(int? year, int page = 1)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            TempData["SelectedYear"] = year;
            int skip = ((int)page - 1) * 10;

            ViewBag.TotalPage = Math.Ceiling(db.SaleImports.Count() / 10.0);
            ViewBag.Page = page;
            List<SalaryReportDetailsDto> salaryReportDetails = new List<SalaryReportDetailsDto>();
            var salaryByUsers = db.CalculatedSalaryByUsers.Where(w => w.Date.Year == year)
                 .GroupBy(g => g.UserId)
                 .Select(s => new {
                     userId = s.Key,
                     userName = s.FirstOrDefault().User.UserName,
                     totalPrice = s.Sum(su => su.Salary),
                     date = s.FirstOrDefault().Date,
                     formulaName = s.FirstOrDefault().User.CalcForum.Name
                 })
                   .OrderBy(d => d.userId)
                 .Skip(skip).Take(10).ToList();

            if (salaryByUsers != null)
            {
                foreach (var item in salaryByUsers)
                {
                    SalaryReportDetailsDto salaryReport = new SalaryReportDetailsDto
                    {

                        UserId = item.userId,
                        UserName = item.userName,
                        FormulaName = item.formulaName,
                        TotalPrice = item.totalPrice,
                        Date = item.date
                    };
                    salaryReportDetails.Add(salaryReport);
                }
                return View(salaryReportDetails);
            }
          

            return RedirectToAction("index");
        }
        [HttpGet]
        public ActionResult SalaryRepoByWorkerDetails(int? year, int? id)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            TempData["SelectedYear"] = year;
            TempData["UserId"] = id;
            
            var sqlMinDate = (DateTime)SqlDateTime.MinValue;
            List<SalaryReportDetailsDto> salaryReportDetails = new List<SalaryReportDetailsDto>();
         
            var SalaryWorkerByMonyh = db.CalculatedSalaryByUsers.Where(w => w.Date.Year == year && w.UserId == id)
              .GroupBy(o => SqlFunctions.DateAdd("month", SqlFunctions.DateDiff("month", sqlMinDate, o.Date), sqlMinDate))
              .Select(s => new {

                  date = s.Key,
                  userName = s.FirstOrDefault().User.UserName,
                  totalPrice = s.Sum(su => su.Salary)

              })
                   .OrderBy(d => d.date.Value.Month).
                ToList();
            if (SalaryWorkerByMonyh != null)
            {
                foreach (var item in SalaryWorkerByMonyh)
                {
                    SalaryReportDetailsDto salaryReport = new SalaryReportDetailsDto
                    {

                        UserName = item.userName,
                        TotalPrice = item.totalPrice,
                        Date = item.date.Value
                    };
                    salaryReportDetails.Add(salaryReport);
                }
                return View(salaryReportDetails);
            }

            return RedirectToAction("index");
        }
        [HttpGet]
        public ActionResult ExportSalaryReportByWorkers()
        {
            int? year = (int)TempData["SelectedYear"];
            if (year == null)
            {
                return RedirectToAction("index");
            }
            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Reports");
            string fileName = "Hesabat.xlsx";

            ws.Row(1).Height = 25;
         

            Dictionary<string, string> items = new Dictionary<string, string>
            {
                {"A","İşçinin Adı" },
                {"B","Düstur" },
                {"C","Maaş" },
                {"D","İl" }
            };

            foreach (KeyValuePair<string, string> item in items)
            {
                ws.Cell(item.Key + "1").Value = item.Value;
                ws.Cell(item.Key + "1").Style.Font.SetFontSize(12);
                ws.Cell(item.Key + "1").Style.Font.SetFontColor(XLColor.FromArgb(255, 255, 255));
                ws.Cell(item.Key + "1").Style.Fill.SetBackgroundColor(XLColor.FromArgb(21, 107, 125));
                ws.Cell(item.Key + "1").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                ws.Cell(item.Key + "1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Column(item.Key).Width = 22;
            }

            var salaryByUsers = db.CalculatedSalaryByUsers.Where(w => w.Date.Year == year)
                            .GroupBy(g => g.UserId)
                            .Select(s => new {
                                userId = s.Key,
                                userName = s.FirstOrDefault().User.UserName,
                                totalPrice = s.Sum(su => su.Salary),
                                date = s.FirstOrDefault().Date,
                                formulaName = s.FirstOrDefault().User.CalcForum.Name
                            }).OrderBy(d => d.userId).ToList();

              int i = 2;
            foreach (var item in salaryByUsers)
            {
                ws.Cell("A" + i).Value = item.userName;
                ws.Cell("B" + i).Value = item.formulaName;
                ws.Cell("C" + i).Value = item.totalPrice;
                ws.Cell("D" + i).Value = item.date.Year;

                i++;
            }
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveAs(stream);

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        [HttpGet]
        public ActionResult ExportSalaryRepoByWorkerDetails()
        {
            int? year = (int)TempData["SelectedYear"];
            int? id = (int)TempData["UserId"];
            if (year == null || id == null)
            {
                return RedirectToAction("index");
            }
            var sqlMinDate = (DateTime)SqlDateTime.MinValue;
            var culture = new System.Globalization.CultureInfo("az");
            User user = db.Users.Find(id);
            if (user == null)
                return null;
            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Reports");
            string fileName = "Hesabat.xlsx";

            ws.Row(1).Height = 25;

            Dictionary<string, string> items = new Dictionary<string, string>
            {
                {"A","Ay" },
                {"B","Məbləğ" }
            };

            foreach (KeyValuePair<string, string> item in items)
            {
                ws.Cell(item.Key + "1").Value = item.Value;
                ws.Cell(item.Key + "1").Style.Font.SetFontSize(12);
                ws.Cell(item.Key + "1").Style.Font.SetFontColor(XLColor.FromArgb(255, 255, 255));
                ws.Cell(item.Key + "1").Style.Fill.SetBackgroundColor(XLColor.FromArgb(21, 107, 125));
                ws.Cell(item.Key + "1").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                ws.Cell(item.Key + "1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Column(item.Key).Width = 22;
            }

            ws.Cell("C" + 1).Value = "İşçinin Adı = " + user.UserName;
            ws.Cell("C" + 1).Style.Font.SetFontSize(12);
            ws.Cell("C" + 1).Style.Font.SetFontColor(XLColor.FromArgb(255, 255, 255));
            ws.Cell("C" + 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(1, 116, 62));
            ws.Cell("C" + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Cell("C" + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Column("C").Width = 22;

            var SalaryWorkerByMonyh = db.CalculatedSalaryByUsers.Where(w => w.Date.Year == year && w.UserId == id)
                       .GroupBy(o => SqlFunctions.DateAdd("month", SqlFunctions.DateDiff("month", sqlMinDate, o.Date), sqlMinDate))
                       .Select(s => new {

                           date = s.Key,
                           totalPrice = s.Sum(su => su.Salary)

                       })
                            .OrderBy(d => d.date.Value.Month).
                         ToList();

            int i = 2;
            foreach (var item in SalaryWorkerByMonyh)
            {
                ws.Cell("A" + i).Value = item.date.Value.ToString("MMMM", culture);
                ws.Cell("B" + i).Value = item.totalPrice.ToString("0.00");
                i++;
            }
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveAs(stream);

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        #endregion

        #region Sale
        [HttpGet]
        public ActionResult SaleReportByMonth(int? year)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }

            List<SalesReportByDateDto> monthDtos = new List<SalesReportByDateDto>();
            var sqlMinDate = (DateTime)SqlDateTime.MinValue;

            var SalesByMonyh = db.Sales.Where(w => w.Date.Year == year).OrderBy(o => o.Date.Month)
                .GroupBy(o => SqlFunctions.DateAdd("month", SqlFunctions.DateDiff("month", sqlMinDate, o.Date), sqlMinDate))
                .Select(s => new {
                   date = s.Key,
                    totalCount = s.Count(),
                    totalDisCount = (int?)s.Where(w => w.DisCount == true).Count() ?? 0,
                    totalVip = (int?)s.Where(w => w.Vip == true).Count() ?? 0,
                    totalIsComfirmed = (int?)s.Where(w => w.IsComfirmed == true).Count() ?? 0,
                    totalNotConfirmed = (int?)s.Where(w => w.IsComfirmed == false).Count() ?? 0,
                    totalImported = (int?)s.Where(w => w.IsComfirmed == true).Count() ?? 0

                }).ToList();

            foreach (var item in SalesByMonyh)
            {
                SalesReportByDateDto monthDto = new SalesReportByDateDto
                {

                    Date = item.date,
                    TotalCount = item.totalCount,
                    TotalDisCount =  item.totalDisCount,
                    TotalIsComfirmed = item.totalIsComfirmed,
                    totalNotConfirmed = item.totalNotConfirmed,
                    TotalImported = item.totalImported,
                    TotalVip = item.totalVip

                };
                monthDtos.Add(monthDto);
            }
            return View(monthDtos);
        }
        [HttpGet]
        public ActionResult SaleReportByQuarterly(int? year)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }

            List<SalesReportByDateDto> monthDtos = new List<SalesReportByDateDto>();
            var sqlMinDate = (DateTime)SqlDateTime.MinValue;

            var SalesByMonyh = db.Sales.Where(w => w.Date.Year == year).OrderBy(o => o.Date.Month)
                .GroupBy(o => (o.Date.Month - 1) / 3 + 1)
                .Select(s => new {

                    quarterly = s.Key,

                    totalCount = s.Count(),
                    totalDisCount = (int?)s.Where(w => w.DisCount == true).Count() ?? 0,
                    totalVip = (int?)s.Where(w => w.Vip == true).Count() ?? 0,
                    totalIsComfirmed = (int?)s.Where(w => w.IsComfirmed == true).Count() ?? 0,
                    totalNotConfirmed = (int?)s.Where(w => w.IsComfirmed == false).Count() ?? 0,
                    totalImported = (int?)s.Where(w => w.IsComfirmed == true).Count() ?? 0

                }).ToList();

            foreach (var item in SalesByMonyh)
            {
                SalesReportByDateDto monthDto = new SalesReportByDateDto
                {

                    quarterly = (byte)item.quarterly,
                    TotalCount = item.totalCount,
                    TotalDisCount = item.totalDisCount,
                    TotalIsComfirmed = item.totalIsComfirmed,
                    totalNotConfirmed = item.totalNotConfirmed,
                    TotalImported = item.totalImported,
                    TotalVip = item.totalVip

                };
                monthDtos.Add(monthDto);
            }
            return View(monthDtos);
        }
        [HttpGet]
        public ActionResult SaleReportByYear(int? year)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }

            List<SalesReportByDateDto> monthDtos = new List<SalesReportByDateDto>();
            var sqlMinDate = (DateTime)SqlDateTime.MinValue;

            var SalesByMonyh = db.Sales.Where(w => w.Date.Year == year).OrderBy(o => o.Date.Month)
                .GroupBy(o => SqlFunctions.DateAdd("year", SqlFunctions.DateDiff("year", sqlMinDate, o.Date), sqlMinDate))
                .Select(s => new {

                    date = s.Key,
                    totalCount = s.Count(),
                    totalDisCount = (int?)s.Where(w => w.DisCount == true).Count() ?? 0,
                    totalVip = (int?)s.Where(w => w.Vip == true).Count() ?? 0,
                    totalIsComfirmed = (int?)s.Where(w => w.IsComfirmed == true).Count() ?? 0,
                    totalNotConfirmed = (int?)s.Where(w => w.IsComfirmed == false).Count() ?? 0,
                    totalImported = (int?)s.Where(w => w.IsComfirmed == true).Count() ?? 0

                }).ToList();

            foreach (var item in SalesByMonyh)
            {
                SalesReportByDateDto monthDto = new SalesReportByDateDto
                {

                    Date = item.date,
                    TotalCount = item.totalCount,
                    TotalDisCount = item.totalDisCount,
                    TotalIsComfirmed = item.totalIsComfirmed,
                    totalNotConfirmed = item.totalNotConfirmed,
                    TotalImported = item.totalImported,
                    TotalVip = item.totalVip

                };
                monthDtos.Add(monthDto);
            }
            return View(monthDtos);
        }
        [HttpGet]
        public ActionResult SaleReportByWorkers(int? year, int page = 1)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            TempData["SelectedSaleYear"] = year;
            int skip = ((int)page - 1) * 10;
            ViewBag.TotalPage = Math.Ceiling(db.SaleImports.Count() / 10.0);
            ViewBag.Page = page;

            List<SaleReportDetailsDto> detailsDtos = new List<SaleReportDetailsDto>();
            var saleByUsers = db.Sales.Where(w => w.Date.Year == year)
             .GroupBy(g => g.UserId)
             .Select(s => new {
                 userId = s.Key,
                 userName = s.FirstOrDefault().User.UserName,
                 date = s.FirstOrDefault().Date,
                 totalCount = s.Count(),
                 totalDisCount = (int?)s.Where(w => w.DisCount == true).Count() ?? 0,
                 totalVip = (int?)s.Where(w => w.Vip == true).Count() ?? 0,
                 totalIsComfirmed = (int?)s.Where(w => w.IsComfirmed == true).Count() ?? 0,
                 totalNotConfirmed = (int?)s.Where(w => w.IsComfirmed == false).Count() ?? 0

             })
               .OrderBy(d => d.userId)
                 .Skip(skip).Take(10).ToList();
            if (saleByUsers != null)
            {
                foreach (var item in saleByUsers)
                {
                    SaleReportDetailsDto detailsDto = new SaleReportDetailsDto
                    {
                        UserId =item.userId,
                        UserName = item.userName,
                        Date = item.date,
                        TotalCount = item.totalCount,
                        TotalDisCount = item.totalDisCount,
                        TotalVip = item.totalVip,
                        TotalIsComfirmed = item.totalIsComfirmed,
                        totalNotConfirmed = item.totalNotConfirmed

                    };
                    detailsDtos.Add(detailsDto);
                }
                return View(detailsDtos);
            }
          
            return RedirectToAction("index");
        }
        [HttpGet]
        public ActionResult SaleRepoByWorkerDetails(int? year, int? id)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            TempData["SelectedSaleYear"] = year;
            TempData["UserId"] = id;
            List<SalesReportByDateDto> monthDtos = new List<SalesReportByDateDto>();
            var sqlMinDate = (DateTime)SqlDateTime.MinValue;

            var SalesByMonyh = db.Sales.Where(w =>w.UserId == id && w.Date.Year == year).OrderBy(o => o.Date.Month)
                .GroupBy(o => SqlFunctions.DateAdd("month", SqlFunctions.DateDiff("month", sqlMinDate, o.Date), sqlMinDate))
                .Select(s => new {
                    date = s.Key,
                    totalCount = s.Count(),
                    totalDisCount = (int?)s.Where(w => w.DisCount == true).Count() ?? 0,
                    totalVip = (int?)s.Where(w => w.Vip == true).Count() ?? 0,
                    totalIsComfirmed = (int?)s.Where(w => w.IsComfirmed == true).Count() ?? 0,
                    totalNotConfirmed = (int?)s.Where(w => w.IsComfirmed == false).Count() ?? 0
                }).ToList();

            foreach (var item in SalesByMonyh)
            {
                SalesReportByDateDto monthDto = new SalesReportByDateDto
                {
                    Date = item.date,
                    TotalCount = item.totalCount,
                    TotalDisCount = item.totalDisCount,
                    TotalIsComfirmed = item.totalIsComfirmed,
                    totalNotConfirmed = item.totalNotConfirmed,
                    TotalVip = item.totalVip
                };
                monthDtos.Add(monthDto);
            }
            return View(monthDtos);
        }
        [HttpGet]
        public ActionResult ExportSaleReportByWorkers()
        {
            int? year = (int?)TempData["SelectedSaleYear"];
            if (year == null)
            {
                return RedirectToAction("index");
            }
            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Reports");
            string fileName = "Satış-Hesabat.xlsx";

            ws.Row(1).Height = 25;


            Dictionary<string, string> items = new Dictionary<string, string>
            {
                {"A","İşçinin Adı" },
                {"B","Sayı" },
                {"C","Endirim" },
                {"D","Vip" },
                {"E", "Təsdiqlənib" },
                {"F", "Təsdiqlənməyib"}
            };

            foreach (KeyValuePair<string, string> item in items)
            {
                ws.Cell(item.Key + "1").Value = item.Value;
                ws.Cell(item.Key + "1").Style.Font.SetFontSize(12);
                ws.Cell(item.Key + "1").Style.Font.SetFontColor(XLColor.FromArgb(255, 255, 255));
                ws.Cell(item.Key + "1").Style.Fill.SetBackgroundColor(XLColor.FromArgb(21, 107, 125));
                ws.Cell(item.Key + "1").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                ws.Cell(item.Key + "1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Column(item.Key).Width = 22;
            }

            var saleByUsers = db.Sales.Where(w => w.Date.Year == year)
            .GroupBy(g => g.UserId)
            .Select(s => new {
                userId = s.Key,
                userName = s.FirstOrDefault().User.UserName,
                date = s.FirstOrDefault().Date,
                totalCount = s.Count(),
                totalDisCount = (int?)s.Where(w => w.DisCount == true).Count() ?? 0,
                totalVip = (int?)s.Where(w => w.Vip == true).Count() ?? 0,
                totalIsComfirmed = (int?)s.Where(w => w.IsComfirmed == true).Count() ?? 0,
                totalNotConfirmed = (int?)s.Where(w => w.IsComfirmed == false).Count() ?? 0

            }).OrderBy(d => d.userId).ToList();

            int i = 2;
            foreach (var item in saleByUsers)
            {
                ws.Cell("A" + i).Value = item.userName;
                ws.Cell("B" + i).Value = item.totalCount;
                ws.Cell("C" + i).Value = item.totalDisCount;
                ws.Cell("D" + i).Value = item.totalVip;
                ws.Cell("E" + i).Value = item.totalIsComfirmed;
                ws.Cell("F" + i).Value = item.totalNotConfirmed;

                i++;
            }
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveAs(stream);

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        [HttpGet]
        public ActionResult ExportSaleRepoByWorkerDetails()
        {
            int? year = (int)TempData["SelectedSaleYear"];
            int? id = (int)TempData["UserId"];
            if(year == null || id == null)
            {
                return  RedirectToAction("index");
            }
            var sqlMinDate = (DateTime)SqlDateTime.MinValue;
            var culture = new System.Globalization.CultureInfo("az");
            User user = db.Users.Find(id);
            if (user == null)
                return null;
            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Reports");
            string fileName = "Satış-Hesabat.xlsx";

            ws.Row(1).Height = 25;

            Dictionary<string, string> items = new Dictionary<string, string>
            {
                {"A","Ay" },
                {"B","Sayı" },
                {"C","Endirim" },
                {"D","Vip" },
                {"E", "Təsdiqlənib" },
                {"F", "Təsdiqlənməyib"}
            };

            foreach (KeyValuePair<string, string> item in items)
            {
                ws.Cell(item.Key + "1").Value = item.Value;
                ws.Cell(item.Key + "1").Style.Font.SetFontSize(12);
                ws.Cell(item.Key + "1").Style.Font.SetFontColor(XLColor.FromArgb(255, 255, 255));
                ws.Cell(item.Key + "1").Style.Fill.SetBackgroundColor(XLColor.FromArgb(21, 107, 125));
                ws.Cell(item.Key + "1").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                ws.Cell(item.Key + "1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Column(item.Key).Width = 22;
            }

            ws.Cell("G" + 1).Value = "İşçinin Adı = " + user.UserName;
            ws.Cell("G" + 1).Style.Font.SetFontSize(12);
            ws.Cell("G" + 1).Style.Font.SetFontColor(XLColor.FromArgb(255, 255, 255));
            ws.Cell("G" + 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(1, 116, 62));
            ws.Cell("G" + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Cell("G" + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Column("G").Width = 22;

            var SalesByMonyh = db.Sales.Where(w => w.UserId == id && w.Date.Year == year).OrderBy(o => o.Date.Month)
                .GroupBy(o => SqlFunctions.DateAdd("month", SqlFunctions.DateDiff("month", sqlMinDate, o.Date), sqlMinDate))
                .Select(s => new {
                    date = s.Key,
                    totalCount = s.Count(),
                    totalDisCount = (int?)s.Where(w => w.DisCount == true).Count() ?? 0,
                    totalVip = (int?)s.Where(w => w.Vip == true).Count() ?? 0,
                    totalIsComfirmed = (int?)s.Where(w => w.IsComfirmed == true).Count() ?? 0,
                    totalNotConfirmed = (int?)s.Where(w => w.IsComfirmed == false).Count() ?? 0
                }).ToList();


            int i = 2;
            foreach (var item in SalesByMonyh)
            {
                ws.Cell("A" + i).Value = item.date.Value.ToString("MMMM", culture);
                ws.Cell("B" + i).Value = item.totalCount;
                ws.Cell("C" + i).Value = item.totalDisCount;
                ws.Cell("D" + i).Value = item.totalVip;
                ws.Cell("E" + i).Value = item.totalIsComfirmed;
                ws.Cell("F" + i).Value = item.totalNotConfirmed;
                i++;
            }
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveAs(stream);

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        #endregion
    }
}