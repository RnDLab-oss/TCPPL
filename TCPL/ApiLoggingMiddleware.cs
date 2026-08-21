using Microsoft.Data.SqlClient;
using Repository.IRepository;
using System.Data;
using System.Diagnostics;

namespace API
{
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IStoredProcedureRepository storedProcedureRepository)
        {
            // ==========================================
            // Request Start
            // ==========================================

            var requestTime = DateTime.Now;

            var stopwatch = Stopwatch.StartNew();

            int? userId = null;
            string? sessionId = null;

            string? errorMessage = null;
            string? errorFileName = null;
            int? errorLineNumber = null;
            string? stackTrace = null;

            try
            {
                // ==========================================
                // JWT se UserId
                // ==========================================

                var userIdClaim =
                    context.User.FindFirst("UserId");

                if (userIdClaim != null &&
                    int.TryParse(
                        userIdClaim.Value,
                        out int parsedUserId))
                {
                    userId = parsedUserId;
                }

                // ==========================================
                // JWT se SessionId
                // ==========================================

                var sessionClaim =
                    context.User.FindFirst("SessionId");

                if (sessionClaim != null &&
                    Guid.TryParse(
                        sessionClaim.Value,
                        out Guid parsedSessionId))
                {
                    sessionId = parsedSessionId.ToString();
                }

                // ==========================================
                // Actual API Execute
                // ==========================================

                await _next(context);
            }
            catch (Exception ex)
            {
                // ==========================================
                // Exception Middleware level par aayi
                // ==========================================

                errorMessage = ex.Message;
                stackTrace = ex.StackTrace;

                GetExceptionLocation(ex,out errorFileName,out errorLineNumber);

                throw;
            }
            finally
            {
                // ==========================================
                // Stop Timer
                // ==========================================

                stopwatch.Stop();

                var responseTime = DateTime.Now;

                // ==========================================
                // Login ke time UserId / SessionId
                // ==========================================

                if (userId == null &&
                    context.Items.TryGetValue(
                        "UserId",
                        out var itemUserId))
                {
                    if (itemUserId != null &&
                        int.TryParse(
                            itemUserId.ToString(),
                            out int parsedUserId))
                    {
                        userId = parsedUserId;
                    }
                }

                if (sessionId == null &&
                    context.Items.TryGetValue(
                        "SessionId",
                        out var itemSessionId))
                {
                    if (itemSessionId != null &&
                        Guid.TryParse(
                            itemSessionId.ToString(),
                            out Guid parsedSessionId))
                    {
                        sessionId = parsedSessionId.ToString();
                    }
                }

                // ==========================================
                // Error Information
                // ==========================================

                if (context.Items.TryGetValue(
                        "ErrorMessage",
                        out var itemErrorMessage))
                {
                    errorMessage =
                        itemErrorMessage?.ToString();
                }

                // ==========================================
                // Exception object
                // GlobalExceptionFilter / Controller
                // se aa sakta hai
                // ==========================================

                if (context.Items.TryGetValue("Exception",out var itemException) && itemException is Exception exception)
                {
                    errorMessage = exception.Message;

                    stackTrace = exception.StackTrace;

                    GetExceptionLocation(exception,out errorFileName,out errorLineNumber);
                }

                // ==========================================
                // Debug
                // ==========================================

                Console.WriteLine(
                    $"ErrorMessage: {errorMessage}"
                );

                Console.WriteLine(
                    $"ErrorFileName: {errorFileName}"
                );

                Console.WriteLine(
                    $"ErrorLineNumber: {errorLineNumber}"
                );

                Console.WriteLine(
                    $"StackTrace: {stackTrace}"
                );

                // ==========================================
                // SQL Parameters
                // ==========================================

                var parameters = new SqlParameter[]
                {
                    new SqlParameter(
                        "@UserId",
                        SqlDbType.Int)
                    {
                        Value = userId.HasValue
                            ? userId.Value
                            : DBNull.Value
                    },

                    new SqlParameter(
                        "@SessionId",
                        SqlDbType.NVarChar,
                        100)
                    {
                        Value = (object?)sessionId
                            ?? DBNull.Value
                    },

                    new SqlParameter(
                        "@ApiName",
                        SqlDbType.NVarChar,
                        500)
                    {
                        Value =
                            context.Request.Path.ToString()
                    },

                    new SqlParameter(
                        "@HttpMethod",
                        SqlDbType.NVarChar,
                        10)
                    {
                        Value =
                            context.Request.Method
                    },

                    new SqlParameter(
                        "@RequestTime",
                        SqlDbType.DateTime)
                    {
                        Value = requestTime
                    },

                    new SqlParameter(
                        "@ResponseTime",
                        SqlDbType.DateTime)
                    {
                        Value = responseTime
                    },

                    new SqlParameter(
                        "@StatusCode",
                        SqlDbType.Int)
                    {
                        Value =
                            context.Response.StatusCode
                    },

                    new SqlParameter(
                        "@ExecutionTimeMs",
                        SqlDbType.BigInt)
                    {
                        Value =
                            stopwatch.ElapsedMilliseconds
                    },

                    new SqlParameter(
                        "@IpAddress",
                        SqlDbType.NVarChar,
                        50)
                    {
                        Value =
                            context.Connection
                                .RemoteIpAddress
                                ?.ToString()
                            ?? (object)DBNull.Value
                    },

                    new SqlParameter(
                        "@ErrorMessage",
                        SqlDbType.NVarChar,
                        -1)
                    {
                        Value =
                            (object?)errorMessage
                            ?? DBNull.Value
                    },

                    new SqlParameter(
                        "@ErrorFileName",
                        SqlDbType.NVarChar,
                        500)
                    {
                        Value =
                            (object?)errorFileName
                            ?? DBNull.Value
                    },

                    new SqlParameter(
                        "@ErrorLineNumber",
                        SqlDbType.Int)
                    {
                        Value =
                            errorLineNumber.HasValue
                                ? errorLineNumber.Value
                                : DBNull.Value
                    },

                    new SqlParameter(
                        "@StackTrace",
                        SqlDbType.NVarChar,
                        -1)
                    {
                        Value =
                            (object?)stackTrace
                            ?? DBNull.Value
                    }
                };

                // ==========================================
                // API LOG
                // ==========================================

                Console.WriteLine(
                    $"API LOG => " +
                    $"UserId: {userId}, " +
                    $"SessionId: {sessionId}, " +
                    $"Api: {context.Request.Path}, " +
                    $"Status: {context.Response.StatusCode}, " +
                    $"Execution: {stopwatch.ElapsedMilliseconds}ms, " +
                    $"Error: {errorMessage}, " +
                    $"File: {errorFileName}, " +
                    $"Line: {errorLineNumber}"
                );

                // ==========================================
                // Stored Procedure
                // ==========================================

                try
                {
                    await storedProcedureRepository
                        .InsertDataScalarAsync(
                            "Usp_InsertLog",
                            parameters);
                }
                catch (Exception logException)
                {
                    Console.WriteLine(
                        $"API LOG ERROR: " +
                        $"{logException.Message}"
                    );
                }
            }
        }

        // ==========================================
        // Exception Location
        // ==========================================

        private static void GetExceptionLocation(
            Exception exception,
            out string? fileName,
            out int? lineNumber)
        {
            fileName = null;
            lineNumber = null;

            var stackTrace =
                new StackTrace(
                    exception,
                    true);

            var frames =
                stackTrace.GetFrames();

            if (frames == null)
                return;

            foreach (var frame in frames)
            {
                var currentFileName =
                    frame.GetFileName();

                var currentLineNumber =
                    frame.GetFileLineNumber();

                if (!string.IsNullOrWhiteSpace(
                        currentFileName) &&
                    currentLineNumber > 0)
                {
                    fileName = currentFileName;
                    lineNumber = currentLineNumber;

                    return;
                }
            }
        }
    }
}