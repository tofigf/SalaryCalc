using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SalaryCalc.Models
{
    public class ButtonsStatic
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(300)]
        public string Name { get; set; }
        [Required, StringLength(300)]
        public string Key { get; set; }

    }
    }