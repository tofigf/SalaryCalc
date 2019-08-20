using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataAccessLayer
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(150)]
        public string Controller { get; set; }
        [MaxLength(150)]
        public string Action { get; set; }
        [MaxLength(150)]
        public string Name { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}