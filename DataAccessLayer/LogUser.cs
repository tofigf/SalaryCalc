using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DataAccessLayer
{
    public class LogUser
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(150)]
        public string OldUserName { get; set; }
        [MaxLength(150)]
        public string OldFullName { get; set; }
        [MaxLength(150)]
        public string OldEmail { get; set; }
        [MaxLength(150)]
        public string OldPhone { get; set; }
        [MaxLength(150)]
        public string OldPinCod { get; set; }

        [ForeignKey("Log")]
        public int LogId { get; set; }

        public virtual Log Log { get; set; }

    }
}