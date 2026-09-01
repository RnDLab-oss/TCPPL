using ERP_API.DTOs;
using ERP_API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace ERP_API.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class MasterController : ControllerBase
        {
        private readonly IReportRepository _report;
        public MasterController(IReportRepository reportRepository)
        {
            _report = reportRepository;
        }

        // ------------------ Create API 

        [HttpPost("GetData")]
        public IActionResult GetData(ReportRequest request)
        {
            DataTable dt = _report.GetData(request);
            return Ok(dt);
        }
    }

}
