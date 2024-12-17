using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MasterNeverDown.TokenBlackList.Filters;

public class JwtAuthenticationFilter : IAsyncActionFilter
{
    private readonly IConfiguration _configuration;

    public JwtAuthenticationFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.IsAllowAnonymous())
        {
            
            await next();
            return;
        }
        var token = context.HttpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(token) || token.Split(" ").Length < 2)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        token = token.Split(" ")[1]; // Bearer {token}

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                },
                out var validatedToken
            );

            context.HttpContext.User = new ClaimsPrincipal(principal);
            await next();
        }
        catch (Exception)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}