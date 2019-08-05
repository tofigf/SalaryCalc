using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SalaryCalc.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required,MaxLength(150)]
        public string UserName { get; set; }
        [Required,MaxLength(150)]
        public string FullName { get; set; }
        [Required,MaxLength(150)]
        public string Email { get; set; }
        [Required,MaxLength(150)]
        public string Password { get; set; }
        public Nullable<int> PostionId { get; set; }

        public virtual Postion Postion { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}