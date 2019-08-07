using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalaryCalc.Models.VwModel
{
    public class VwRoles
    {
        public  List<Postion> Postions { get; set; }
        public List<Role> Roles { get; set; }
        public List<UserRole> UserRoles  { get; set; }
    }
}