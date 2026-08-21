using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel
{
    public class LoginRequestDto
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        //public string LastName { get; set; }
        public string Role { get; set; }
        public string UserPhone { get; set; }
        public string UserEmail { get; set; }
        public DateTime Expiry { get; set; }
    }
}
