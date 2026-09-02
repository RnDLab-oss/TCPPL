namespace ERP_API.DTOs
{
    public class TaskListRequest
    {
        public int? DepartmentID { get; set; }
        public string? TaskType { get; set; }
        public string? Frequency { get; set; }
        public string? Priority { get; set; }
        public int? UserID { get; set; }
        public string? StatusKey { get; set; }
        public string? Search { get; set; }
        public string? Tab { get; set; } = "all";
        public DateTime? AsOfDate { get; set; }
    }
}
