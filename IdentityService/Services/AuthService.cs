using IdentityService.Interfaces;
using IdentityService.Models;
using IdentityService.Models.Login;
using IdentityService.Models.Register;
using IdentityService.Models.Revoke;
using IdentityService.Rules;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityService.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly ITokenService tokenService;
        private readonly AuthRules authRules;
        private readonly RoleManager<Role> roleManager;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(UserManager<User> userManager, IConfiguration configuration, ITokenService tokenService, AuthRules authRules, RoleManager<Role> roleManager, IHttpContextAccessor httpContextAccessor, ITokenBlacklistService tokenBlacklistService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.tokenService = tokenService;
            this.authRules = authRules;
            this.roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _tokenBlacklistService = tokenBlacklistService;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            User? user = await userManager.FindByEmailAsync(loginRequest.Email);
            if (user != null)
            {
                bool checkPassword = await userManager.CheckPasswordAsync(user, loginRequest.Password);
                await authRules.EmailOrPasswordShouldNotBeInvalid(user, checkPassword);
            }
            else
            {
                await authRules.EmailOrPasswordShouldNotBeInvalid(user, false);
            }

            IList<string> roles = await userManager.GetRolesAsync(user);

            JwtSecurityToken token = await tokenService.CreateToken(user, roles);
            string refreshToken = tokenService.GenerateRefreshToken();

            _ = int.TryParse(configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

            await userManager.UpdateAsync(user);
            await userManager.UpdateSecurityStampAsync(user);

            string _token = new JwtSecurityTokenHandler().WriteToken(token);

            await userManager.SetAuthenticationTokenAsync(user, "Default", "AccessToken", _token);

            return new()
            {
                Token = _token,
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            };
        }

        public async Task Register(RegisterRequest registerRequest)
        {
            await authRules.UserShouldNotBeExist(await userManager.FindByEmailAsync(registerRequest.Email));

            User user = new User()
            {
                Email = registerRequest.Email,
                Fullname = registerRequest.FullName,
            };
            user.UserName = registerRequest.Email;
            user.SecurityStamp = Guid.NewGuid().ToString();

            IdentityResult result = await userManager.CreateAsync(user, registerRequest.Password);
            if (result.Succeeded)
            {
                if (!await roleManager.RoleExistsAsync("user"))
                    await roleManager.CreateAsync(new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = "user",
                        NormalizedName = "USER",
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                    });

                await userManager.AddToRoleAsync(user, "user");
            }
        }

        public async Task Revoke(RevokeRequest revokeRequest)
        {
            User user = await userManager.FindByEmailAsync(revokeRequest.Email);
            await authRules.EmailAddressShouldBeValid(user);

            user.RefreshToken = null;
            await userManager.UpdateAsync(user);
        }

        public async Task RevokeAll()
        {
            List<User> users = await userManager.Users.ToListAsync();

            foreach (User user in users)
            {
                user.RefreshToken = null;
                await userManager.UpdateAsync(user);
            }
        }

        public async Task RevokeAccessToken(RevokeAccessTokenRequest revokeAccessTokenRequest)
        {
            string? accessToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorizedAccessException("Access token tapılmadı.");
            }

            accessToken = accessToken.Replace("Bearer ", "").Trim();

            TimeSpan expiryTime = TimeSpan.FromMinutes(60);
            await _tokenBlacklistService.AddToBlacklist(accessToken, expiryTime);
        }
    }
}
