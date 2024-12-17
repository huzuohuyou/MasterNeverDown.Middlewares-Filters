namespace MasterNeverDown.TokenBlackList.Middlewares;

/// <summary>
/// Token black list service
/// </summary>
public interface ITokenBlacklistService
{
    /// <summary>
    /// Add token to black list with expiration time
    /// </summary>
    /// <param name="token">token</param>
    public void BlacklistToken(string token);
    /// <summary>
    /// The token is in black list
    /// </summary>
    /// <param name="token">token</param>
    /// <returns>The token is in black list</returns>
    bool IsTokenBlacklisted(string token);
}