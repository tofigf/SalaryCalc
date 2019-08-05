using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SalaryCalc.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }
        public double Price { get; set; }
        public int MyProperty { get; set; }
        [Required, MaxLength(150)]
        public string Name { get; set; }
        public bool Vip { get; set; }
        public bool DisCount { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}