using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel
{
    public class AdminTaskStatusDTO
    {
        public int TaskId { get; set; }

        public string AdminStatus { get; set; }

        public string? AdminRemarks { get; set; }
    }
}
