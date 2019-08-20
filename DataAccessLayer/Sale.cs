using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DataAccessLayer
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }
        public double Price { get; set; }
     
        [Required, MaxLength(150)]
        public string Name { get; set; }
        public bool Vip { get; set; }
        public bool DisCount { get; set; }
        public int Count { get; set; }
        public bool IsComfirmed { get; set; }
        public bool IsImported { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("SaleImport")]
        public int? SaleImportId { get; set; }

        public virtual SaleImport SaleImport { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Log> Logs { get; set; }

    }
}