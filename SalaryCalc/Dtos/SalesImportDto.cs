using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalaryCalc.Dtos
{
    public class SalesImportDto
    {
        public string UserName { get; set; }
        public string ExcelName { get; set; }
        public string FileUrl { get; set; }
    }
}