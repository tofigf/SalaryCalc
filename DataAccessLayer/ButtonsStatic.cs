using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
       public partial  class ButtonsStatic
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(300)]
        public string Name { get; set; }
        [Required, StringLength(300)]
        public string Key { get; set; }
    }
}
