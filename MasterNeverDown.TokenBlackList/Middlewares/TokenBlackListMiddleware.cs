using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MasterNeverDown.TokenBlackList.Middlewares;

/// <summary>
/// Middleware to validate JWT tokens and check if they are blacklisted.
/// </summary>
public class TokenBlackListMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITokenBlacklistService _tokenBlacklistService;
    private readonly ILogger<TokenBlackListMiddleware>? _logger;
    private readonly string? _path;


    /// <summary>
    /// Initializes a new instance of the <see cref="TokenBlackListMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="tokenBlacklistService">Service to check if a token is blacklisted.</param>
    /// <param name="logger">Logger to log information.</param>
    public TokenBlackListMiddleware(RequestDelegate next, ITokenBlacklistService tokenBlacklistService,
        ILogger<TokenBlackListMiddleware> logger)
    {
        _next = next;
        _tokenBlacklistService = tokenBlacklistService;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to validate the token.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Log the execution of the middleware
        _logger?.LogInformation("Executing TokenValidationMiddleware");

        // Retrieve the token from the Authorization header
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrWhiteSpace(token))
        {
            await _next(context);
            return;
        }

        // Check if the token is blacklisted
        if (_tokenBlacklistService.IsTokenBlacklisted(token)
            && !context.Request.Path.Equals(_path ?? "/Account/login"))
        {
            // If the token is blacklisted, set the response status to 401 Unauthorized
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        // Call the next middleware in the pipeline
        await _next(context);
    }
}