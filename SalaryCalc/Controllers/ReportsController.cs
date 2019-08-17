using SalaryCalc.Dtos;
using SalaryCalc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalaryCalc.Controllers
{
    public class ReportsController : BaseController
    {

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }
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
                .GroupBy(o => SqlFunctions.DateAdd("month", SqlFunctions.DateDiff("month", sqlMinDate, o.Date), sqlMinDate))
                .Select(s => new {

                    quarterly = ((s.Key.Value.Month - 1) / 3 + 1),

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
            int skip = ((int)page - 1) * 10;

            ViewBag.TotalPage = Math.Ceiling(db.SaleImports.Count() / 10.0);
            ViewBag.Page = page;
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
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
    }
}