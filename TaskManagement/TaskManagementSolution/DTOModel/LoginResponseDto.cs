using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel
{
    public class LoginResponseDtos
    {
        public string Role { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string Token { get; set; }

        public string UserEmail { get; set; }

        public string UserPhone { get; set; }

        public Guid SessionId { get; set; }
    }
}
