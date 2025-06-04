using Tradof.Common.Enums;

namespace Tradof.Auth.Services.DTOs
{
    public class LoginResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
        public long? CompanyId { get; set; }
        public GroupName? GroupName { get; set; }
        public string JobTitle { get; set; }
        public CompanyUserType? CompanyUserType { get; set; }
    }
}
