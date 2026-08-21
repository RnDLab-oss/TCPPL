using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel
{
    public class EditTaskDTO
    {
        public int TaskId { get; set; }

        public string TaskTitle { get; set; }

        public string? TaskDescription { get; set; }

        public int AssignedEmployeeId { get; set; }

        public DateTime Deadline { get; set; }

        public string Priority { get; set; }

        public string EmployeeStatus { get; set; }

        public string? EmployeeRemarks { get; set; }

        public string AdminStatus { get; set; }

        public string? AdminRemarks { get; set; }
    }
}
