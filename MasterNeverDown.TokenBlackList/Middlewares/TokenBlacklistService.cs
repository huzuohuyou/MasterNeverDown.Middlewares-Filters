using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;

namespace MasterNeverDown.TokenBlackList.Middlewares;

/// <summary>
/// Token black list service
/// </summary>
public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly IMemoryCache _cache;

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="cache"></param>
    public TokenBlacklistService(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Add token to black list
    /// </summary>
    /// <param name="token">token</param>
    public void BlacklistToken(string token)
    {
        
        var dt =GetTokenExpiration(token);
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = dt
        };

        _cache.Set(token, true, cacheEntryOptions);
    }

    private DateTime? GetTokenExpiration(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

        var expClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp);
        if (expClaim == null)
            return null;

        var exp = long.Parse(expClaim.Value);
        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;

        return expirationTime;
    }
    
    /// <summary>
    /// The token is in black list
    /// </summary>
    /// <param name="token">token</param>
    /// <returns>The token is in black list</returns>
    public bool IsTokenBlacklisted(string token)
    {
        return _cache.TryGetValue(token, out _);
    }
}