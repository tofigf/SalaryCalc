using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SalaryCalc.Models
{
    public class CalculatedSalaryByUser
    {
        public int Id { get; set; }
        public double Salary { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("CalcForum")]
        public int CalcForumId { get; set; }
        public DateTime Date { get; set; }

        public virtual User User { get; set; }
        public virtual CalcForum CalcForum { get; set; }
    }
}