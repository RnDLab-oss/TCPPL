namespace ERP_API.DTOs
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public int UserRoleId { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Token { get; set; }
        public string UserToken { get; set; }
    }
}
