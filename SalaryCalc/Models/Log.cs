using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SalaryCalc.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; }
        public int  CurrentUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required, MaxLength(250)]
        public string Method { get; set; }
        public int? ActionedUserId { get; set; }
        public int? SaleId { get; set; }

        [ForeignKey("ActionedUserId")]
        public virtual User ActionedUser { get; set; }
        [ForeignKey("CurrentUserId")]
        public virtual User CurrentUser { get; set; }
        [ForeignKey("SaleId")]
        public virtual Sale Sale { get; set; }
        public virtual ICollection<LogUser> LogUsers { get; set; }
        public virtual ICollection<LogSale> LogSales { get; set; }
    }
}