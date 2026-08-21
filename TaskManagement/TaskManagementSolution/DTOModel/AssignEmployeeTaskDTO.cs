using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel
{
    public class AssignEmployeeTaskDTO
    {
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }

        public int AssignedEmployeeId { get; set; }
        public int AssignedBy { get; set; }

        public DateTime Deadline { get; set; }

        public string Priority { get; set; }
    }
}
