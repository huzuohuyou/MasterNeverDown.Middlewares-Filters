using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MasterNeverDown.TokenBlackList.Filters;

public static class FilterExtensions
{
    public static bool IsAllowAnonymous(this ActionExecutingContext context)
    {
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        if (actionDescriptor != null)
        {
            var allowAnonymous = actionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                .Any(attr => attr.GetType() == typeof(AllowAnonymousAttribute));
            return allowAnonymous;
        }
        return false;
    }
}