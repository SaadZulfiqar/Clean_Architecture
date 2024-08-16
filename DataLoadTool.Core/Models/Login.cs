namespace DataLoadTool.Core.Models
{
    public class Login
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Tenant_id { get; set; } = string.Empty;
        public bool IsSuperUser { get; set; } = false;
        public string Token { get; set; } = string.Empty;
    }
}
