using System.ComponentModel.DataAnnotations;

namespace ERP_API.DTOs
{
    public class ReportRequest
    {
        public int? CompId { get; set; }
        public int? BranchId { get; set; }
        public int? AcYear { get; set; }
        public int? UserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? RptType { get; set; }
    }

    public class DashboardKpiRequest
    {
        public int CompId { get; set; }
        public int BranchId { get; set; }
        public int AcYear { get; set; }
        public int Userid { get; set; }
        public int RptType { get; set; }
        public int ViewPeriod { get; set; }
        public string UserToken { get; set; }
    }


    public class GetDataRequest
    {
        public int CompId { get; set; }
        public int BranchId { get; set; }
        public int AcYear { get; set; }
        [Required(ErrorMessage = "RptType is required.")]
        public int? RptType { get; set; }
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
    }

    public class UserpermissionRequest
    {
        public int CompId { get; set; }
        public int BranchId { get; set; }
        public int AcYear { get; set; }
        public int RoleId { get; set; }
        public string UserToken { get; set; } = string.Empty;
    }


}
