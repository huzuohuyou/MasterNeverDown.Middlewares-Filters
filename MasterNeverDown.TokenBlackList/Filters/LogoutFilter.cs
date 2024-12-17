using MasterNeverDown.TokenBlackList.Middlewares;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace MasterNeverDown.TokenBlackList.Filters;

public class LogoutFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Retrieve the token from the Authorization header
        var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        
        if (!string.IsNullOrWhiteSpace(token))
        {
            // Get the token blacklist service from the service provider
            var tokenBlacklistService = context.HttpContext.RequestServices.GetService<ITokenBlacklistService>();
            
            // Blacklist the token
            tokenBlacklistService?.BlacklistToken(token);
        }

        // Proceed with the next action in the filter pipeline
        await next();
    }
}