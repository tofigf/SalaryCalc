using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalaryCalc.Dtos
{
    public class SalesGroupFileDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserFullname { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string ProductName { get; set; }
        public double? TotalPrice { get; set; }
        public double? TotalCount { get; set; }

    }
}