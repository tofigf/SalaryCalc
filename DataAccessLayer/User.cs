﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DataAccessLayer
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
        [Required, MaxLength(150)]
        public string Phone { get; set; }
        [MaxLength(150)]
        public string Password { get; set; }

        [RegularExpression(@"^[A-Z0-9]+$")]
        [Required, StringLength(7, MinimumLength = 7)]
        public string PinCod { get; set; }
       
        [ForeignKey("Postion")]
        public Nullable<int> PostionId { get; set; }

        [ForeignKey("CalcForum")]
        public Nullable<int> CalcForumId { get; set; }

        public virtual Postion Postion { get; set; }
        public virtual CalcForum CalcForum { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }

        [InverseProperty("ActionedUser")]
        public virtual ICollection<Log> ActionedUserLogs { get; set; }
        [InverseProperty("CurrentUser")]
        public virtual ICollection<Log> CurrentLogs { get; set; }
        public virtual ICollection<CalculatedSalaryByUser> CalculatedSalaryByUsers { get; set; }
        public virtual ICollection<LogCalcSalary> LogCalcSalaries { get; set; }
    }
}