using System;
using System.Collections.Generic;

namespace Model;

public partial class TaskManagement
{
    public int TaskId { get; set; }

    public string TaskTitle { get; set; } = null!;

    public string? TaskDescription { get; set; }

    public int AssignedEmployeeId { get; set; }

    public string AssignedBy { get; set; } = null!;

    public DateTime AssignedDate { get; set; }

    public DateTime Deadline { get; set; }

    public DateTime? CompletionDate { get; set; }

    public string EmployeeStatus { get; set; } = null!;

    public string? EmployeeRemarks { get; set; }

    public string AdminStatus { get; set; } = null!;

    public string? AdminRemarks { get; set; }
    public string? Priority { get; set; }
    public int DelayDays { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual EmployeeMaster AssignedEmployee { get; set; } = null!;
}
