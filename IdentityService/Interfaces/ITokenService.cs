using IdentityService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityService.Interfaces
{
    public interface ITokenService
    {
        Task<JwtSecurityToken> CreateToken(User user, IList<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
    }
}
