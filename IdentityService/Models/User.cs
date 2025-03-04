using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models
{
    public class User : IdentityUser<Guid>
    {
        public string Fullname { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
