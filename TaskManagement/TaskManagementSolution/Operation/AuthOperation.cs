using DTOModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Model;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Operation
{
    public class AuthOperation : IAuthOperation
    {
        private readonly EmpTaskMsDbContext _context;
        private readonly IConfiguration _configuration;
        //private readonly IAuthRepository _authRepository;
        private readonly ILogger<AuthOperation> _logger;

        public AuthOperation(
            EmpTaskMsDbContext context,
            //IAuthRepository authRepository,
            IConfiguration configuration,
            ILogger<AuthOperation> logger)
        {
            _context = context;
            _configuration = configuration;
            //_authRepository = authRepository;
            _logger = logger;
        }

        //public async Task<ApiResponse> Login(LoginRequestDto dto)
        //{
        //    try
        //    {
        //        var user = await _context.EmployeeMasters
        //            .FirstOrDefaultAsync(x => x.ContactNumber == dto.PhoneNumber);

        //        if (user == null)
        //        {
        //            return new ApiResponse("200", false, new int[0], "Invalid Phone Number.");
        //        }

        //        //bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);

        //        //if (!isPasswordValid)
        //        //{
        //        //    return new ApiResponse("200", false, new int[0], "Invalid Password.");
        //        //}

        //        if (dto.Password != user.Password)
        //        {
        //            return new ApiResponse("200", false, new int[0], "Invalid Password.");
        //        }

        //        //var userRole = await _context.Roles
        //        //    .FirstOrDefaultAsync(r => r.Id == user.RoleId);

        //        //if (userRole == null)
        //        //{
        //        //    return new ApiResponse("200", false, new int[0], "User role not found.");
        //        //}

        //        var token = await GenerateToken(user.ContactNumber);

        //        var verified = new LoginResponseDto
        //        {
        //            Role = user.Role,
        //            EmployeeId = user.EmployeeId,
        //            EmployeeName = user.EmployeeName,
        //            Token = token,
        //            UserEmail = user.Email,
        //            UserPhone = user.ContactNumber
        //        };

        //        return new ApiResponse("200", true, verified, "Login successful.");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred during login. Phone: {Phone}", dto.PhoneNumber);
        //        throw;
        //    }
        //}



        //------------Orignal Login Api Start---------------------
        public async Task<ApiResponse> Login(LoginRequestDto dto)
        {
            try
            {
                var constr = _configuration.GetConnectionString("defaultString");

                using SqlConnection con = new SqlConnection(constr);

                using SqlCommand cmd = new SqlCommand("Usp_LoginEmployee", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ContactNumber", dto.PhoneNumber);
                cmd.Parameters.AddWithValue("@Password", dto.Password);

                await con.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return new ApiResponse(
                        "200",
                        false,
                        new int[0],
                        "Login failed."
                    );
                }

                int flag = Convert.ToInt32(reader["Flag"]);

                // Phone Number not found
                if (flag == -1)
                {
                    return new ApiResponse(
                        "200",
                        false,
                        new int[0],
                        "Invalid Phone Number."
                    );
                }

                // Password incorrect
                if (flag == -2)
                {
                    return new ApiResponse(
                        "200",
                        false,
                        new int[0],
                        "Invalid Password."
                    );
                }

                // Login successful
                if (flag == 1)
                {
                    var employeeId = Convert.ToInt32(reader["EmployeeId"]);
                    var employeeName = reader["EmployeeName"]?.ToString();
                    var contactNumber = reader["ContactNumber"]?.ToString();
                    var email = reader["Email"]?.ToString();
                    var role = reader["Role"]?.ToString();

                    var token = await GenerateToken(contactNumber);

                    var verified = new LoginResponseDto
                    {
                        Role = role,
                        EmployeeId = employeeId,
                        EmployeeName = employeeName,
                        Token = token,
                        UserEmail = email,
                        UserPhone = contactNumber
                    };

                    return new ApiResponse(
                        "200",
                        true,
                        verified,
                        "Login successful."
                    );
                }

                return new ApiResponse(
                    "200",
                    false,
                    new int[0],
                    "Invalid login."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred during login. Phone: {Phone}",
                    dto.PhoneNumber
                );

                return new ApiResponse(
                    "500",
                    false,
                    new int[0],
                    "An error occurred during login."
                );
            }
        }

        public async Task<string> GenerateToken(string phone)
        {
            try
            {
                var user = await _context.EmployeeMasters
                    .FirstOrDefaultAsync(x => x.ContactNumber == phone);

                if (user == null)
                {
                    throw new Exception("User not found while generating token.");
                }

                //var userRole = await _context.Roles
                //    .FirstOrDefaultAsync(r => r.Id == user.RoleId);

                //if (userRole == null)
                //{
                //    throw new Exception("User role not found while generating token.");
                //}

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.MobilePhone, phone),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Name, user.EmployeeName),
                    new Claim("EmployeeId", user.EmployeeId.ToString())
                };

                var jwtSecret = _configuration["JWT:Secret"];

                if (string.IsNullOrWhiteSpace(jwtSecret))
                {
                    throw new Exception("JWT Secret key is missing in appsettings.json.");
                }

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSecret)
                );

                var credentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(
                        Convert.ToDouble(_configuration["JWT:ExpirationMinute"])
                    ),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating token. Phone: {Phone}", phone);
                throw;
            }
        }

        //------------Orignal Login Api End---------------------



    }
}
