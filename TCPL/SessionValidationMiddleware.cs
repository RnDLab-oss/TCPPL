using Microsoft.Data.SqlClient;
using Repository.IRepository;
using System.Data;

namespace API
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IStoredProcedureRepository _IStoredProcedureRepository;

        public SessionValidationMiddleware(RequestDelegate next, IStoredProcedureRepository IStoredProcedureRepository)
        {
            _next = next;
            _IStoredProcedureRepository = IStoredProcedureRepository;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IStoredProcedureRepository repository)
        {
            // JWT authentication check
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim =
                    context.User.FindFirst("UserId")?.Value;

                var sessionIdClaim =
                    context.User.FindFirst("SessionId")?.Value;

                // Claim missing
                if (!int.TryParse(userIdClaim, out int userId) ||
                    !Guid.TryParse(sessionIdClaim, out Guid sessionId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                // Validate session from DB
                var parameters = new[]
                {
                //new SqlParameter("@UserId" , userId),
                //new SqlParameter("@SessionId", sessionId)
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId },
                  new SqlParameter("@SessionId", SqlDbType.VarChar) { Value = Convert.ToString(sessionId) }
            };

                var ds = await _IStoredProcedureRepository.GetDataSetAsync("Usp_ValidateUserSession",parameters);

                bool isValid =
                    ds != null &&
                    ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0 &&
                    Convert.ToBoolean(ds.Tables[0].Rows[0]["IsValid"]);

                if (!isValid)
                {
                    context.Response.StatusCode =
                        StatusCodes.Status401Unauthorized;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Session expired or invalid."
                    });

                    return;
                }
            }

            await _next(context);
        }
    }
}
