using CalcSalaryApi.Data.Repository.Interface;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CalcSalaryApi.Data.Repository
{
    public class ReportRepository: IReportRepository
    {
        private readonly DataContext _context;
        public ReportRepository(DataContext context)
        {
            _context = context;
        }

        public async Task <List<SalaryReportByDateDto>> SalaryReportEachUserByMonth(int? year,int?id)
            {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            List<SalaryReportByDateDto> monthDtos = new List<SalaryReportByDateDto>();
            var sqlMinDate = (DateTime)SqlDateTime.MinValue;

            var SalaryByMonyh = await  _context.CalculatedSalaryByUsers.Where(w => w.Date.Year == year && w.UserId == id).OrderBy(o => o.Date.Month)
                .GroupBy(o => SqlFunctions.DateAdd("month", SqlFunctions.DateDiff("month", sqlMinDate, o.Date), sqlMinDate))
                .Select(s => new
                {
                    date = s.Key,
                    total = s.Sum(su => su.Salary)

                }).ToListAsync();

            foreach (var item in SalaryByMonyh)
            {
                SalaryReportByDateDto monthDto = new SalaryReportByDateDto
                {

                    Date = item.date,
                    Total = item.total

                };
                monthDtos.Add(monthDto);
            }

            return monthDtos;
        }
        public async Task<List<SalaryReportByMonthDto>> SalaryReportByMonth(int? year)
        {
            var culture = new System.Globalization.CultureInfo("az");
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            List<SalaryReportByMonthDto> monthDtos = new List<SalaryReportByMonthDto>();
            var sqlMinDate = (DateTime)SqlDateTime.MinValue;

            var SalaryByMonyh = await _context.CalculatedSalaryByUsers.Where(w => w.Date.Year == year).OrderBy(o => o.Date.Month)
                .GroupBy(o => SqlFunctions.DateAdd("month", SqlFunctions.DateDiff("month", sqlMinDate, o.Date), sqlMinDate))
                .Select(s => new
                {
                    date = s.Key,
                    total = s.Sum(su => su.Salary),
                }).ToListAsync();

            foreach (var item in SalaryByMonyh)
            {
                SalaryReportByMonthDto monthDto = new SalaryReportByMonthDto
                {
                    Month = item.date.Value.ToString("MMMM", culture),
                    Total = item.total,
                };
                monthDtos.Add(monthDto);
            }

            return monthDtos;
        }



    }
}