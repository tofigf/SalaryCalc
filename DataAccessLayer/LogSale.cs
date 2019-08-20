using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DataAccessLayer
{
    public partial class LogSale
    {
        [Key]
        public int Id { get; set; }

        public double OldPrice { get; set; }
        [MaxLength(150)]
        public string OldName { get; set; }
        public bool OLdVip { get; set; }
        public bool OLdDisCount { get; set; }
        public int OldCount { get; set; }
        public bool OldIsComfirmed { get; set; }
        public bool OldIsImported { get; set; }


        [ForeignKey("Log")]
        public int LogId { get; set; }

        public virtual Log Log { get; set; }

    }
}