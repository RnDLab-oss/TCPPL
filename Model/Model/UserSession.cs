using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{

    public partial class UserSession
    {
        public long SessionId { get; set; }

        public long UserId { get; set; }

        public string SessionToken { get; set; } = null!;

        public DateTime LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

        public DateTime? ExpiryTime { get; set; }

        public bool IsActive { get; set; }
    }

}
