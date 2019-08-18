using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalaryCalc.Dtos
{
    public class SaleReportDetailsDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int TotalCount { get; set; }
        public DateTime Date { get; set; }
        public int? TotalDisCount { get; set; }
        public int? TotalVip { get; set; }
        public int? TotalIsComfirmed { get; set; }
        public int? totalNotConfirmed { get; set; }
    }
}