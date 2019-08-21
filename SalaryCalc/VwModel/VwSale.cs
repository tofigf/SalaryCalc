using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalaryCalc.VwModel
{
    public class VwSale
    {
        public List<Sale> Sales { get; set; }
        public SearchSaleDto SearchSaleDto { get; set; }
    }
}