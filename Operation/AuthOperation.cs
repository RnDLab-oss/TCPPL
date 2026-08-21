using DTOModel.LoginDTO;
using DTOModel.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Model;
using Operation.IOperation;
using Repository;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Operation
{
    public class AuthOperation : IAuthOperation
    {
        private readonly IStoredProcedureRepository _IStoredProcedureRepository;
        private readonly TcpplWebContext _context;
        private readonly IConfiguration _configuration;
        //private readonly IAuthRepository _authRepository;
        private readonly ILogger<AuthOperation> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthOperation(
            TcpplWebContext context,
            //IAuthRepository authRepository,
            IConfiguration configuration,
            IStoredProcedureRepository IStoredProcedureRepository,
              IHttpContextAccessor httpContextAccessor,
            ILogger<AuthOperation> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _IStoredProcedureRepository = IStoredProcedureRepository;
            _context = context;
            _configuration = configuration;
            //_authRepository = authRepository;
            _logger = logger;
        }
       

        public async Task<ApiResponse> Login(Login_Req_Dto dto)
        {
            try
            {
                var parameter = new SqlParameter[] 
                {
                  new SqlParameter("@Phone",SqlDbType.VarChar){Value = dto.PhoneNumber },
                  new SqlParameter("@Password",SqlDbType.VarChar){Value =dto.Password }
                 
                };

                var result = await _IStoredProcedureRepository.GetDataSetAsync("Usp_LoginEmployee", parameter);

               
                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    
                    return new ApiResponse(401, false, new int[0], "Login failed.");
                }

                DataRow row = result.Tables[0].Rows[0];

                int flag = Convert.ToInt32(row["Flag"]);

                if (flag == -1)
                {
                    return new  ApiResponse(401, false, new int[0], "Invalid phone number.");
                }

                if (flag == -2)
                {
                    return new  ApiResponse(401, false, new int[0], "Invalid password.");
                }

                if (flag == -3)
                {
                    return new ApiResponse(403, false, new int[0], "Inactive user.");
                }

                if (flag == 1)
                {

                    var token = await GenerateToken(dto.PhoneNumber);


                    // ==========================================
                    // API Logging Middleware ke liye
                    // ==========================================

                    _httpContextAccessor.HttpContext!.Items["UserId"] = Convert.ToInt32(row["UserID"]);

                    _httpContextAccessor.HttpContext.Items["SessionId"] =
                        token.SessionToken;

                    var response = new Login_Resp_Dto
                    {
                        UserId = Convert.ToInt32(row["UserID"]),
                        UserName = row["UserName"]?.ToString(),
                        PhoneNumber = row["Phone"]?.ToString(),
                        EmailId = row["EmailId"]?.ToString(),
                        Token = token.JWTToken,
                        SessionToken = token.SessionToken,
                    };

                    return new ApiResponse(200, true, response, "Login Successfull"); 
                }

                return new ApiResponse(401, false, new int[0], "Login failed.");
            }
            catch (Exception ex)
            {
                
                return new ApiResponse(500, false, new int[0], "An error occurred during logging."+ex.Message);
            }
        }


        public async Task<TokenResultDto> GenerateToken(string Phone)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.Phone == Phone);

                if (user == null)
                {
                    throw new Exception("User not found while generating token.");
                }

                var jwtSecret = _configuration["JWT:Secret"];

                if (string.IsNullOrWhiteSpace(jwtSecret))
                {
                    throw new Exception(
                        "JWT Secret key is missing in appsettings.json."
                    );
                }

                // ==========================================
                // 1. Generate Session ID
                // ==========================================

                var sessionToken = Guid.NewGuid().ToString();

                // ==========================================
                // 2. Expiry Time
                // ==========================================

                var expirationMinutes = Convert.ToDouble(
                    _configuration["JWT:ExpirationMinute"]
                );

                var expiryTime = DateTime.Now.AddMinutes(expirationMinutes);

                // ==========================================
                // 3. JWT Claims
                // ==========================================

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.MobilePhone,user.Phone ?? ""),

                    new Claim(ClaimTypes.Email,user.EmailId ?? ""),

                    new Claim(ClaimTypes.Name,user.UserName ?? ""),

                    new Claim("UserId",user.UserId.ToString()),

                    // Session ID JWT ke andar
                    new Claim("SessionId",sessionToken)
                };

                // ==========================================
                // 4. JWT Key
                // ==========================================

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSecret)
                );

                var credentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );

                // ==========================================
                // 5. Generate JWT
                // ==========================================

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    claims: claims,
                    expires: expiryTime,
                    signingCredentials: credentials
                );

                var jwtToken = new JwtSecurityTokenHandler()
                    .WriteToken(token);

              
                var parameter = new SqlParameter[]
               {
                  new SqlParameter("@UserId",SqlDbType.Int){Value = user.UserId },
                  new SqlParameter("@SessionToken",SqlDbType.VarChar){Value = sessionToken },
                  new SqlParameter("@LoginTime",SqlDbType.DateTime){Value =DateTime.Now },
                  new SqlParameter("@ExpiryTime",SqlDbType.DateTime){Value =expiryTime }

               };

                var Id = await _IStoredProcedureRepository.InsertDataScalarAsync("Usp_StoreSession", parameter);

               var sessionData= await _context.UserSessions.Where(x=>x.SessionId==Convert.ToInt32(Id)).FirstOrDefaultAsync();

                // ==========================================
                // 7. Return Token + SessionId
                // ==========================================

                return new TokenResultDto
                {
                    SessionToken = sessionData.SessionToken,
                    JWTToken = jwtToken
                };
            }
            catch (Exception ex)
            {
                //_logger.LogError(
                //    ex,
                //    "Error occurred while generating token. UserId: {UserId}",
                //    userId
                //);

                throw;
            }
        }

    }
}
