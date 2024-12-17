using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MasterNeverDown.LoginLimit.Middlewares;

/// <summary>
/// Middleware to limit login attempts and lock out users after a certain number of failed attempts.
/// </summary>
public class LoginLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoginLimitMiddleware> _logger;
    private readonly Stopwatch _stopwatch;
    private readonly IMemoryCache _cache;
    private const int MaxRetryAttempts = 3;
    private const string RetryCacheKeyPrefix = "LoginRetry_";
    private readonly string? _path;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginLimitMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="cache">The memory cache instance.</param>
    public LoginLimitMiddleware(RequestDelegate next, ILogger<LoginLimitMiddleware> logger,
        IConfiguration configuration, IMemoryCache cache)
    {
        _next = next;
        _logger = logger;
        _cache = cache;
        _stopwatch = new Stopwatch();
    }

    /// <summary>
    /// Middleware invocation method.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public async Task Invoke(HttpContext context)
    {
        _stopwatch.Restart();
        var request = context.Request;
        //filter request path
        if (!request.Path.StartsWithSegments(_path ?? DefaultPath.LoginPath))
        {
            await _next(context);
            return;
        }

        var username = "";
        // get request body content
        if (request.Method.ToLower().Equals("post"))
        {
            request.EnableBuffering();

            var stream = request.Body;
            if (request.ContentLength is > 0)
            {
                var buffer = new byte[request.ContentLength.Value];
                _ = await stream.ReadAsync(buffer, 0, buffer.Length);
                var json = Encoding.UTF8.GetString(buffer);
                using var document = JsonDocument.Parse(json);
                username = document.RootElement.GetProperty("username").GetString();
            }

            request.Body.Position = 0;
        }

        var cacheKey = $"{RetryCacheKeyPrefix}{username}";

        if (_cache.TryGetValue(cacheKey, out int retryCount) && retryCount >= MaxRetryAttempts)
        {
            _logger.LogWarning($"User {username} is locked out due to too many failed login attempts.");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Account locked due to too many failed login attempts.");
            return;
        }

        // 响应完成记录时间和存入日志
        context.Response.OnCompleted(() =>
        {
            _stopwatch.Stop();
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                _cache.TryGetValue(cacheKey, out retryCount);
                retryCount++;
                _cache.Set(cacheKey, retryCount, TimeSpan.FromMinutes(15));
                _logger.LogInformation($"Failed login attempt {retryCount} for user {username}.");
            }
            else if (context.Response.StatusCode == StatusCodes.Status200OK)
            {
                _cache.Remove(cacheKey);
            }

            return Task.CompletedTask;
        });
        await _next(context);
    }

    /// <summary>
    /// 获取响应内容
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <returns>A task that represents the completion of reading the response content.</returns>
    public async Task<string> GetResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        return text;
    }
}