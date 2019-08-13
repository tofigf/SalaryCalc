using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SalaryCalc.Models
{
    public class SaleImport
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string FileUrl { get; set; }
        public virtual List<Sale> Sales { get; set; }
    }
}