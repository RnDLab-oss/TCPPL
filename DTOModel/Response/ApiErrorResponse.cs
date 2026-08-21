using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.Response
{
    public class ApiErrorResponse : ApiResponse
    {
        public string StatusCode { get; set; }


        public ApiErrorResponse(string statusCode, string errorMessage)
            : base(false, null, errorMessage)
        {
            this.StatusCode = statusCode;
        }

        public ApiErrorResponse(string statusCode, string errorMessage, object data)
            : base(false, data, errorMessage)
        {
            this.StatusCode = statusCode;
        }

        public ApiErrorResponse(Exception ex)
            : base(false, null, ex.Message)
        {
            this.StatusCode = "500"; // Default to internal server error
        }

        public ApiErrorResponse(int statusCode, bool success, string errorMessage) : base(statusCode, success, errorMessage)
        {
            Status = statusCode;
            Success = success;
            Message = errorMessage;
        }

        public ApiErrorResponse(int statusCode, bool success, object data, string errorMessage) : base(statusCode, success, data, errorMessage)
        {
            Status = statusCode;
            Success = success;
            Data = data;
            Message = errorMessage;
        }
    }
}
