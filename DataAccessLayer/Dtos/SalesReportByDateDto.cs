using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAccessLayer
{
    public class SalesReportByDateDto
    {
        public DateTime? Date { get; set; }
        public int TotalCount { get; set; }
        public int? TotalDisCount { get; set; }
        public int? TotalVip { get; set; }
        public int? TotalIsComfirmed { get; set; }
        public int? totalNotConfirmed { get; set; }
        public int? TotalImported { get; set; }
        public byte? quarterly { get; set; }
    }
}