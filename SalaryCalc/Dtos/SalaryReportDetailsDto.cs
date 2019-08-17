using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalaryCalc.Dtos
{
    public class SalaryReportDetailsDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FormulaName { get; set; }
        public double TotalPrice { get; set; }
        public DateTime Date { get; set; }
    }
}