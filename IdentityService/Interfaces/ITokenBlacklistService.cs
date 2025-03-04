namespace IdentityService.Interfaces
{
    public interface ITokenBlacklistService
    {
        Task AddToBlacklist(string token, TimeSpan expiry);
        Task<bool> IsTokenRevoked(string token);
    }
}
