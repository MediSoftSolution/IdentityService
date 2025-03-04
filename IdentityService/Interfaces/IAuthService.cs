using IdentityService.Models.Login;
using IdentityService.Models.Register;
using IdentityService.Models.Revoke;

namespace IdentityService.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest loginRequest);
        Task Register(RegisterRequest registerRequest);
        Task Revoke(RevokeRequest revokeRequest);
        Task RevokeAll();
        Task RevokeAccessToken(RevokeAccessTokenRequest revokeAccessTokenRequest);
    }
}
