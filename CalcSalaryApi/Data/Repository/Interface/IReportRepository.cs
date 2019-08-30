using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CalcSalaryApi.Data.Repository.Interface
{
    public interface IReportRepository
    {
       Task <List<SalaryReportByDateDto>> SalaryReportEachUserByMonth(int? year,int? id);
        Task<List<SalaryReportByMonthDto>> SalaryReportByMonth(int? year);
        
    }
}