using API.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace API
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILoggerManager _logger;

        public GlobalExceptionFilter(ILoggerManager logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            // ==========================================
            // Exception message ApiLoggingMiddleware
            // ke liye HttpContext.Items me store karo
            // ==========================================

            context.HttpContext.Items["ErrorMessage"] =
                context.Exception.Message;

            // Optional: complete exception bhi store kar sakte ho
            context.HttpContext.Items["Exception"] =
                context.Exception;

            // ==========================================
            // Logging
            // ==========================================

            _logger.LogError(
                $"Application thrown error: {context.Exception.Message}"
            );

            // ==========================================
            // Response
            // ==========================================

            context.HttpContext.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            context.HttpContext.Response.ContentType =
                "application/json";

            context.Result = new JsonResult(new
            {
                success = false,
                message = context.Exception.Message
            });

            // Exception handled
            context.ExceptionHandled = true;
        }
    }
}