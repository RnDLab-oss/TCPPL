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
        public class CommonController : ControllerBase
        {
        private readonly ICommonRepository _get;
        public CommonController(ICommonRepository commonRepository)
        {
            _get = commonRepository;
        }

        // ------------------ Create API 

        [HttpPost("GetDataById")]
        public IActionResult GetDataById(GetDataRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _get.GetDataById(request);
            return Ok(result);
        }

        [HttpPost("GetDropdown")]
        public IActionResult GetDropdown(GetDropdownRequest request)
        {
            var result = _get.GetDropdown(request);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }


        [HttpPost("GetUserPermissions")]
        public IActionResult GetUserPermissions(UserpermissionRequest request)
        {
            var result = _get.GetUserPermissions(request);
            return Ok(result);
        }

    }

}
