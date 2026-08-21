using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelTCPL
{
    public class EmployeeSession
    {
        public Guid SessionId { get; set; }

        public int EmployeeId { get; set; }

        public string? Token { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

        public bool IsActive { get; set; }

        public DateTime? ExpiryTime { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
