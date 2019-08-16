using SalaryCalc.Dtos;
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
    }
}