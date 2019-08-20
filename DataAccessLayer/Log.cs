using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DataAccessLayer
{
    public class Log
    {
        [Key]
        public int Id { get; set; }
        public int  CurrentUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UsedDate { get; set; }
        [Required, MaxLength(250)]
        public string Action { get; set; }
        public string Controller { get; set; }
        public int? ActionedUserId { get; set; }
        [ForeignKey("ActionedUserId")]
        public virtual User ActionedUser { get; set; }
        [ForeignKey("CurrentUserId")]
        public virtual User CurrentUser { get; set; }
     
        public virtual ICollection<LogUser> LogUsers { get; set; }
        public virtual ICollection<LogSale> LogSales { get; set; }
        public virtual ICollection<LogCalcForum> LogCalcForums { get; set; }
    }
}