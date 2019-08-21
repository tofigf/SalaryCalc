using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
  public  class SearchSaleDto
    {
        public string Name { get; set; }
        public string SaleName { get; set; }
        public DateTime? Date { get; set; }
        public string Price { get; set; }
        public bool? IsComfirmed { get; set; }
    }
}
