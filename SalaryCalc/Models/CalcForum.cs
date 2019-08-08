using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SalaryCalc.Models
{
    public class CalcForum
    {
        public int Id { get; set; }

        [Required,StringLength(500)]
        public string Formula { get; set; }
        public DateTime Date { get; set; }

        [Required,StringLength(200)]
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }

    }
}