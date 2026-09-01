using ERP_API.DTOs;
using ERP_API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace ERP_API.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class TransactionController : ControllerBase
        {
        private readonly IReportRepository _report;
        public TransactionController(IReportRepository reportRepository)
        {
            _report = reportRepository;
        }

        // ------------------ Create API 
    }

}
