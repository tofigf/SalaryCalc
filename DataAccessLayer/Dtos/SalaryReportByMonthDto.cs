﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
  public  class SalaryReportByMonthDto
    {
        public string Month { get; set; }
        public double? Total { get; set; }
        public byte Quarterly { get; set; }
    }
}
