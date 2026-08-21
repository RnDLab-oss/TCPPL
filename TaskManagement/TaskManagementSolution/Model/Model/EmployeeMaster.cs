using System;
using System.Collections.Generic;

namespace Model;

public partial class EmployeeMaster
{
    public int EmployeeId { get; set; }

    public string? EmployeeName { get; set; }

    public string? ContactNumber { get; set; }

    public string? Email { get; set; }

    public int? Department { get; set; }

    public DateOnly? JoiningDate { get; set; }

    public bool? Status { get; set; }

    public string? Role { get; set; }

    public string? Password { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<TaskManagement> TaskManagements { get; set; } = new List<TaskManagement>();
}
