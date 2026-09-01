namespace ERP_API.DTOs
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Count { get; set; }
        public int Id { get; set; }
        public object Data { get; set; }
    }

    public class GetDropdownRequest
    {
        public int CompId { get; set; }
        public int BranchId { get; set; }
        public int AcYear { get; set; }
        public string FormKey { get; set; }

    }

    public class DashboardKpiResponse
    {
        public DashboardSummary Summary { get; set; }

        public DashboardTaskStatus TaskStatus { get; set; }

        public List<object> MonthlyTrend { get; set; }

        public List<DepartmentPerformance> DepartmentPerformance { get; set; }
    }

    public class DashboardSummary
    {
        public int TotalTasks { get; set; }
        public int Completed { get; set; }
        public int Pending { get; set; }
        public int Overdue { get; set; }
        public int ExtensionsUsed { get; set; }

        public decimal CompletionRate { get; set; }
        public decimal PendingRate { get; set; }
        public decimal OverdueRate { get; set; }
    }
    public class DashboardTaskStatus
    {
        public int Completed { get; set; }
        public int Pending { get; set; }
        public int Overdue { get; set; }
    }
    public class DepartmentPerformance
    {
        public int DepartmentID { get; set; }

        public string DepartmentName { get; set; }

        public int Assigned { get; set; }

        public int Completed { get; set; }

        public decimal CompletionRate { get; set; }
    }
}
