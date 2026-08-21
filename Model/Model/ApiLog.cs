using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{

    public partial class ApiLog
    {
        public long Id { get; set; }

        public int? UserId { get; set; }

        public string? Session { get; set; }

        public string? ApiName { get; set; }

        public string? HttpMethod { get; set; }

        public DateTime? RequestTime { get; set; }

        public DateTime? ResponseTime { get; set; }

        public int? StatusCode { get; set; }

        public long? ExecutionTimeMs { get; set; }

        public string? IpAddress { get; set; }

        public string? ErrorMessage { get; set; }

        public string? ErrorFileName { get; set; }

        public int? ErrorLineNumber { get; set; }

        public string? StackTrace { get; set; }
    }

}
