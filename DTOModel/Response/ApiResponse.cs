using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace DTOModel.Response
{
    public class ApiResponse
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }

        public ApiResponse(object data)
        {
            Success = true;
            Data = data;
            Message = "Request was successful.";
        }

        //public ApiResponse(object data,decimal i)
        //{            
        //    Data = data;
        //}

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? Extra { get; set; }

        public ApiResponse(object data, decimal i)
        {
            // Copy all properties from the anonymous object into Extra
            var json = JsonSerializer.Serialize(data);
            Extra = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        }

        public ApiResponse(object data, string message)
        {
            Success = true;
            Data = data;
            Message = message;
        }

        public ApiResponse(bool success, object data, string message)
        {
            Success = success;
            Data = data;
            Message = message;
        }

        public ApiResponse(int statusCode, string messageText)
        {
            Status = statusCode;
            Message = messageText;
        }

        public ApiResponse(int statusCode, bool success, object data, string message)
        {
            Status = statusCode;
            Success = success;
            Data = data;
            Message = message;
        }

        public ApiResponse(int statusCode, bool success, object data)
        {
            Status = statusCode;
            Success = success;
            Data = data;

        }
    }
}
