using CalcSalaryApi.Data.Repository.Interface;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace CalcSalaryApi.Controllers
{
    [RoutePrefix("api/reports")]
    public class ReportsController : ApiController
    {
        private readonly IReportRepository _repo;
        public ReportsController(IReportRepository repo)
        {
            _repo = repo;
        }
        // GET: Reports

        [HttpGet]
        [Route("salaryreportbymonth")]
        //[ResponseType(typeof(SalaryReportByDateDto))]
        public IHttpActionResult SalaryReportByMonth()
        {
            int? year = 2019;
            if (year == null)
                return StatusCode(HttpStatusCode.BadGateway);
            if (_repo.SalaryReportByMonth(year) == null)
                return BadRequest();

            var SalaryReportByMonth = _repo.SalaryReportByMonth(year);
            return Ok(SalaryReportByMonth);
        }

    }
}