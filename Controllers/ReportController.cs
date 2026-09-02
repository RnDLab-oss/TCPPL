using ERP_API.DTOs;
using ERP_API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace ERP_API.Controllers
{
    [Authorize]
    [ApiController]
        [Route("api/[controller]")]
        public class ReportController : ControllerBase
        {
        private readonly IReportRepository _report;
        public ReportController(IReportRepository reportRepository)
        {
            _report = reportRepository;
        }

        // ------------------ Create API 

        [HttpPost("GetData")]
        public IActionResult GetData(ReportRequest request)
        {
            var result = _report.GetData(request);
            return Ok(result);
        }


        [HttpPost("DashboardKpi")]
        public IActionResult GetDashboardKpi(DashboardKpiRequest request)
        {
            var result = _report.GetDashboardKpi(request);
            return Ok(result);
        }

        [HttpPost("TaskList")]
        public IActionResult TaskList(TaskListRequest request)
        {
            var result = _report.GetTaskList(request);
            return Ok(result);
        }


    }

}
