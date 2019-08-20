using CalcSalaryApi.Data.Repository.Interface;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace CalcSalaryApi.Controllers
{
    [RoutePrefix("api/reports")]
    [Authorize]
    public class ReportsController : ApiController
    {
        private readonly IReportRepository _repo;
        public ReportsController(IReportRepository repo)
        {
            _repo = repo;
        }
        // GET: Reports

        [HttpGet]
        [Route("salaryreportbymonth/{year}")]
        [ResponseType(typeof(SalaryReportByDateDto))]
        public async Task <IHttpActionResult> SalaryReportByMonth(int? year)
        {
            int? currentUserId = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value);
            if (year == null)
                return StatusCode(HttpStatusCode.BadGateway);
            if (await _repo.SalaryReportByMonth(year,currentUserId) == null)
                return BadRequest();

            var SalaryReportByMonth = await _repo.SalaryReportByMonth(year,currentUserId);
            return Ok(SalaryReportByMonth);
        }

    }
}