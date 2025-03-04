using IdentityService.Interfaces;
using StackExchange.Redis;

namespace IdentityService.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly IDatabase _redisDb;

        public TokenBlacklistService(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task AddToBlacklist(string token, TimeSpan expiry)
        {
            await _redisDb.StringSetAsync(token, "revoked", expiry);
        }

        public async Task<bool> IsTokenRevoked(string token)
        {
            return await _redisDb.KeyExistsAsync(token);
        }
    }
}
