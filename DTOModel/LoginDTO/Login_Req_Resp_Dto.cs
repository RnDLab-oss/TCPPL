using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.LoginDTO
{
    public class Login_Req_Dto
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }

    public class TokenResultDto
    {
        public string SessionToken { get; set; }

        public string JWTToken { get; set; } = null!;
    }
    public class Login_Resp_Dto
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string SessionToken { get; set; }
        public string Token { get; set; }
    }
}
