namespace ERP_API.DTOs
{
    public class SaveTaskRequest
    {
        public string Mode { get; set; }
        public string UserToken { get; set; } = string.Empty;
        public int EntryNo { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string TaskType { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public int AssignEmpId { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Reminder { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class SaveRoleReq
    {
        public int Cmid { get; set; }
        public long BranchID { get; set; }
        public string UserToken { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public long RoleID { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class SaveRolePermissionReq
    {
        public int Cmid { get; set; }
        public long RoleID { get; set; }
        public List<RolePermissionItem> Permissions { get; set; } = new();
    }
    public class RolePermissionItem
    {
        public long PermissionID { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanApprove { get; set; }
        public bool CanPrint { get; set; }
        public bool CanExport { get; set; }
    }
}