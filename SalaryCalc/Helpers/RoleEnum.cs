using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalaryCalc.Helpers
{
    public static class  RoleEnum
    {
        public  enum AdminRole
        {
            None ,
            IsAdmin ,
            IsModerator,
            IsWorker

        }  
    }
}