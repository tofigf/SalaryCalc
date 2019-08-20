using DataAccessLayer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalaryCalc.Models.VwModel
{
    public class VwSalaryReports
    {
        public List<SalaryReportDetailsDto> SalaryReportDetailsDtos { get; set; }
        public SearchDto SearchDto { get; set; }
    }
}