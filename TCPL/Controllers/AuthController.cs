using DTOModel.Response;
using DTOModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DTOModel.LoginDTO;
using Operation.IOperation;
using Model;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : BaseController
    {

        private readonly IConfiguration _configuration;
        private readonly IAuthOperation _authOperation;
        private readonly TcpplWebContext _tcpplWebContext;
        private readonly ILogger<AuthController> logger;

        public AuthController(IConfiguration configuration, TcpplWebContext tcpplWebContext, IAuthOperation authOperation, ILogger<AuthController> logger) : base(logger)
        {
            _configuration = configuration;
            this.logger = logger;
            _authOperation = authOperation;
            _tcpplWebContext = tcpplWebContext;
        }

        [HttpPost("Login")]

        [AllowAnonymous]
        public async Task<ApiResponse> Login([FromForm] Login_Req_Dto loginDTO)
        {
            try
            {
                return await _authOperation.Login(loginDTO);

            }
            catch (Exception ex)
            {
                return new ApiResponse(500, false, new int[0], "An error occurred during process.");
            }
        }



        [HttpPost("GetAllUser")]

        //[AllowAnonymous]
        public async Task<ApiResponse> GetAllUser()
        {
            try
            {
                var data = await _tcpplWebContext.Users.ToListAsync();
                return new ApiResponse(200, true, data, "Found All User");

            }
            catch (Exception ex)
            {
                return new ApiResponse(500, false, new int[0], "An error occurred during process.");
            }
        }

        //For Testing Purpose
        //[HttpPost("GetAllUser")]
        //public async Task<ApiResponse> GetAllUser()
        //{
        //    try
        //    {
        //        // Intentionally exception for testing
        //        throw new Exception("Test exception: GetAllUser API");

        //        var data = await _tcpplWebContext.Users.ToListAsync();

        //        return new ApiResponse(200, true, data, "Found All User");
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.Items["ErrorMessage"] = ex.Message;
        //        HttpContext.Items["Exception"] = ex;
        //        HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        //        return new ApiResponse(
        //            500,
        //            false,
        //            new int[0],
        //            "An error occurred during process."
        //        );
        //    }
    
    }
}
