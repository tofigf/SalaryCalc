using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DataAccessLayer
{
    public class LogCalcSalary
    {
        public int Id { get; set; }
        public double OldSalary { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("Log")]
        public int LogId { get; set; }

        public virtual Log Log { get; set; }
        public virtual User User { get; set; }
    }
}