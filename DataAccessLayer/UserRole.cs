using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAccessLayer
{
    public class UserRole
    {
        public int Id { get; set; }
        public int PostionId { get; set; }
        public int RoleId { get; set; }
        public Nullable<DateTime> Date { get; set; }

        public virtual Postion Postion { get; set; }
        public virtual Role Role { get; set; }
    }
}