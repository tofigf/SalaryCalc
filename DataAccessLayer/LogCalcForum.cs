using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DataAccessLayer
{
    public class LogCalcForum
    {
        public int Id { get; set; }

        [ StringLength(500)]
        public string OldFormula { get; set; }

        [StringLength(200)]
        public string OldName { get; set; }

        [ForeignKey("Log")]
        public int LogId { get; set; }

        public virtual Log Log { get; set; }
    }
}