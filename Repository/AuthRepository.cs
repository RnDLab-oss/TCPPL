    using ERP_API.DTOs;
    using ERP_API.Helpers;
    using ERP_API.Repositories;
    using Microsoft.Data.SqlClient;
    using System.Data;

    namespace ERP_API.Repositories
    {
        // Create Interface for ReportRepository
        public interface IAuthRepository
        {
            LoginResponse Login(LoginRequest request);
        }

        // Implement the interface in the class ----------------------------------------
        public class AuthRepository : IAuthRepository
        {
            private readonly IConfiguration _configuration;
            private readonly DBHelper _db;
            private readonly JwtHelper _jwtHelper;

            public AuthRepository(IConfiguration configuration, JwtHelper jwtHelper)
            {
                _configuration = configuration;
                _db = new DBHelper(_configuration);
                _jwtHelper = jwtHelper;
            }

            public LoginResponse Login(LoginRequest request)
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@UserName", request.Username),
                    new SqlParameter("@Password", request.Password)
                };
            
                DataTable dt = _db.ExecuteDataTable("udp_Web_ValidateLogin", param);
                LoginResponse _res = new LoginResponse();

                if (dt.Rows.Count > 0)
                {
                    _res.Success = pvtUtilitiy.ToBool(dt.Rows[0]["Success"]);
                    _res.Message = pvtUtilitiy.ToString( dt.Rows[0]["Message"]);

                    if (_res.Success)
                    {
                        _res.UserId = pvtUtilitiy.ToInt(dt.Rows[0]["UserId"]);
                        _res.UserName = pvtUtilitiy.ToString(dt.Rows[0]["UserName"]);
                        _res.FullName = pvtUtilitiy.ToString(dt.Rows[0]["FullName"]);
                        _res.UserRole = pvtUtilitiy.ToString(dt.Rows[0]["UserRole"]);
                        _res.UserRoleId = pvtUtilitiy.ToInt(dt.Rows[0]["RoleId"]);
                        _res.Email = pvtUtilitiy.ToString(dt.Rows[0]["Email"]);
                        _res.MobileNo = pvtUtilitiy.ToString(dt.Rows[0]["MobileNo"]);
                        _res.Department = pvtUtilitiy.ToString(dt.Rows[0]["DepartmentName"]);
                        _res.Token = _jwtHelper.GenerateToken(_res.UserId, _res.UserName);
                        _res.UserToken = pvtUtilitiy.ToString(dt.Rows[0]["UserToken"]);
                    }
                }
                return _res;
            }

        }
    }