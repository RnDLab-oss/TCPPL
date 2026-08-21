using DTOModel.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [ServiceFilter(typeof(GlobalExceptionFilter))]
    public class BaseController : ControllerBase
    {
        private readonly ILogger<BaseController> logger;

        public BaseController(ILogger<BaseController> logger)
        {
            this.logger = logger;
        }

        protected ApiResponse CreateResponse1(object data)
        {
            this.logger.LogInformation("Logging started in Base");
            return new ApiResponse(data);
        }

        //protected ApiErrorResponse CreateResponse1(Exception ex)
        //{
        //    this.logger.LogError(Newtonsoft.Json.JsonConvert.SerializeObject(ex));
        //    return new ApiErrorResponse("500", ex.Message); // Assuming a default status code for errors
        //}

        protected ApiErrorResponse CreateResponse1(Exception ex, object data)
        {
            var e = new ApiErrorResponse("500", ex.Message, data); // Assuming a default status code for errors
            return e;
        }
    }
}
