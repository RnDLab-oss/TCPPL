using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel
{
    public class EmployeeTaskStatusDTO
    {
        public int TaskId { get; set; }

        public string EmployeeStatus { get; set; }

        public string? EmployeeRemarks { get; set; }
    }
}
