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
        public class SaveController: ControllerBase
        {

        private readonly ISaveRepository _saveData;
        public SaveController(ISaveRepository saveRepository)
        {
            _saveData = saveRepository;
        }

        // ------------------ Create API 
        [HttpPost("Task")]
        public IActionResult Task(SaveTaskRequest request)
        {
            var result = _saveData.SaveTask(request);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("RoleMaster")]
        public IActionResult RoleMaster(SaveRoleReq request)
        {
            var result = _saveData.RoleMaster(request);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }


        [HttpPost("RolePermission")]
        public IActionResult RolePermission(SaveRolePermissionReq request)
        {
            var result = _saveData.RolePermission(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }



    }
}
