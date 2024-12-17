using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace MasterNeverDown.LoginLimit.Filters;

public class LoginLimitFilter : IAsyncActionFilter
{
    private readonly IMemoryCache _cache;
    private const int MaxAttempts = 3;
    private const int LockoutDuration = 15; // in minutes

    public LoginLimitFilter(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("login", out dynamic? loginObj) )
        {
            var cacheKey = $"LoginAttempts_{loginObj.Username}";
            if (_cache.TryGetValue(cacheKey, out int attempts) && attempts >= MaxAttempts)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 403,
                    Content = "Account is locked due to multiple failed login attempts. Please try again later."
                };
                return;
            }

            var resultContext = await next();

            if (resultContext.Result is UnauthorizedResult)
            {
                _cache.TryGetValue(cacheKey, out attempts);
                _cache.Set(cacheKey, ++attempts, TimeSpan.FromMinutes(LockoutDuration));
            }
            else
            {
                _cache.Remove(cacheKey);
            }
        }
        else
        {
            await next();
        }
    }
}