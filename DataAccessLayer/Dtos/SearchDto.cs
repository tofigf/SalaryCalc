using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAccessLayer
{
    public class SearchDto
    {
        public int? Year { get; set; }
        public string Key { get; set; }
        public string SalaryKey { get; set; }
        public string FormulaName { get; set; }
    }
}