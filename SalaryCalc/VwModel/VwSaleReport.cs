using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalaryCalc.VwModel
{
    public class VwSaleReport
    {
        public List<SaleReportDetailsDto> SaleReportDetailsDtos { get; set; }
        public SearchDto SearchDto { get; set; }
    }
}