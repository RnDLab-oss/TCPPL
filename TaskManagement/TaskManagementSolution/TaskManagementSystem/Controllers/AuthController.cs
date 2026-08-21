using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DTOModel;
using Operation;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthOperation _auth;
        public AuthController(IConfiguration configuration, IAuthOperation auth)
        {
            _configuration = configuration;
            _auth = auth;
        }
        [HttpPost("Login")]

        [AllowAnonymous]
        public async Task<ApiResponse> Login([FromForm] LoginRequestDto loginDTO)
        {
            try
            {
                return await _auth.Login(loginDTO);

            }
            catch (Exception ex)
            {
                return new ApiResponse("500", false, new int[0], "An error occurred during process.");
            }
        }

        //[HttpGet("GetLoginYears")]
        //public async Task<IActionResult> GetLoginYears()
        //{
        //    var response = await _yourService.GetLoginYears();

        //    return Ok(response);
        //}
    }
}
