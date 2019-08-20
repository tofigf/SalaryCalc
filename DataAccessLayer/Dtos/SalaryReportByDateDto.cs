using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAccessLayer
{
    public class SalaryReportByDateDto
    {
        public DateTime? Date { get; set; }
        public double? Total { get; set; }
        public byte Quarterly { get; set; }
    }
}