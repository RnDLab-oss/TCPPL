using ERP_API.DTOs;
using ERP_API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;   

namespace ERP_API.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class AuthController : ControllerBase
        {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        // ------------------ Create API 

        [HttpPost("Login")]
        public IActionResult Login(LoginRequest model)
        {
            var result = _authRepository.Login(model);
            if (!result.Success)
                return Unauthorized(result);
            return Ok(result);
        }


    }

}
