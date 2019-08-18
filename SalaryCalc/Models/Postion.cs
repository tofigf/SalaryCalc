using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SalaryCalc.Models
{
    public class Postion
    {
        [Key]
        public int Id { get; set; }
        [Required,MaxLength(150)]
        public string Name { get; set; }
        public bool Status { get; set; }
        public bool IsAdmin { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}