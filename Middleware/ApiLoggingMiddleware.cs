using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ERP_API.Helpers;
using System.Security.Claims;

namespace ERP_API.Middleware
{
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiLoggingMiddleware(
            RequestDelegate next,
            IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            DateTime requestTime = DateTime.Now;

            string requestBody = await ReadRequestBody(context);

            int statusCode = 200;
            string responseBody = "";

            string? errorMessage = null;
            string? errorFileName = null;
            int? errorLineNumber = null;
            string? stackTrace = null;

            try
            {
                // Original response stream save
                var originalResponseBody = context.Response.Body;

                using var responseMemoryStream = new MemoryStream();

                // Temporary response stream
                context.Response.Body = responseMemoryStream;
                // API execute
                await _next(context);
                stopwatch.Stop();
                statusCode = context.Response.StatusCode;
                // Read response
                responseMemoryStream.Position = 0;

                responseBody = await new StreamReader(responseMemoryStream).ReadToEndAsync();
                // Response back to client
                responseMemoryStream.Position = 0;
                await responseMemoryStream.CopyToAsync(
                    originalResponseBody
                );

                context.Response.Body = originalResponseBody;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                statusCode = 500;
                errorMessage = ex.Message;
                stackTrace = ex.StackTrace;

                if (ex.TargetSite != null)
                {
                    errorFileName = ex.TargetSite.DeclaringType?.FullName;
                }
                throw;
            }
            finally
            {
                DateTime responseTime = DateTime.Now;
                stopwatch.Stop();

                long? userId = null;

                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long id))
                    {
                        userId = id;
                    }
                }

                // JWT Token
                string? sessionToken = null;
                var authorization =
                    context.Request.Headers["Authorization"].FirstOrDefault();

                if (!string.IsNullOrEmpty(authorization))
                {
                    sessionToken = authorization.Replace("Bearer ", "").Trim();
                }

                string apiName =  context.Request.Path.ToString();
                string httpMethod =context.Request.Method;
                string? ipAddress = context.Connection.RemoteIpAddress?.ToString();
                string? deviceName = context.Request.Headers["User-Agent"].FirstOrDefault();

                // Procedure Name
                string? dbObjectName = null;
                if (context.Items.TryGetValue("DbObjectName", out var dbObject))
                {
                    dbObjectName = dbObject?.ToString();
                }

                //Console.WriteLine("========== API LOGGING ==========");
                //Console.WriteLine($"API: {context.Request.Path}");
                //Console.WriteLine($"Method: {context.Request.Method}");
                //Console.WriteLine($"Status: {context.Response.StatusCode}");
                //Console.WriteLine($"Execution: {stopwatch.ElapsedMilliseconds}");
                //Console.WriteLine("=================================");

                // Save log
                await InsertApiLog(
                    userId,
                    sessionToken,
                    apiName,
                    httpMethod,
                    requestTime,
                    requestBody,
                    responseTime,
                    responseBody,
                    statusCode,
                    stopwatch.ElapsedMilliseconds,
                    ipAddress,
                    errorMessage,
                    errorFileName,
                    errorLineNumber,
                    stackTrace,
                    deviceName,
                    dbObjectName
                );
            }
        }

        private async Task<string> ReadRequestBody(HttpContext context)
        {
            context.Request.EnableBuffering();

            context.Request.Body.Position = 0;

            using var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                leaveOpen: true
            );

            string body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            return body;
        }

        private async Task InsertApiLog(
            long? userId,string? sessionToken,string apiName,string httpMethod,DateTime requestTime,string request,
            DateTime responseTime,string response,int statusCode,long executionTimeMs,string? ipAddress,string? errorMessage,
            string? errorFileName,int? errorLineNumber,string? stackTrace,string? deviceName,string? dbObjectName)
        {
            try
            {
                DBHelper db = new DBHelper(_configuration);
                SqlParameter[] param =
                {
                    new SqlParameter("@UserId", userId.HasValue ? userId.Value : DBNull.Value),
                    new SqlParameter("@SessionToken", (object?)sessionToken ?? DBNull.Value),
                    new SqlParameter("@ApiName", apiName),
                    new SqlParameter("@HttpMethod",httpMethod),
                    new SqlParameter("@RequestTime",requestTime),
                    new SqlParameter("@Request",(object?)request ?? DBNull.Value),
                    new SqlParameter("@ResponseTime",responseTime),
                    new SqlParameter("@Response",(object?)response ?? DBNull.Value),
                    new SqlParameter("@StatusCode",statusCode),
                    new SqlParameter("@ExecutionTimeMs",executionTimeMs),
                    new SqlParameter("@IpAddress",(object?)ipAddress ?? DBNull.Value),
                    new SqlParameter("@ErrorMessage",(object?)errorMessage ?? DBNull.Value),
                    new SqlParameter("@ErrorFileName",(object?)errorFileName ?? DBNull.Value),
                    new SqlParameter("@ErrorLineNumber",(object?)errorLineNumber ?? DBNull.Value),
                    new SqlParameter("@StackTrace",(object?)stackTrace ?? DBNull.Value),
                    new SqlParameter("@DeviceName",(object?)deviceName ?? DBNull.Value),
                    new SqlParameter("@DbObjectName",(object?)dbObjectName ?? DBNull.Value),
                    new SqlParameter("@ActionTime", DateTime.Now)
                };
                db.ExecuteNonQuery( "Udp_Web_InsertApiLog", param);
            }
            catch
            {
                
            }
        }
    }
}