using Microsoft.Extensions.DependencyInjection;

namespace MasterNeverDown.TokenBlackList.Middlewares;

public static class AddTokenBlackListExtension
{
    public static void AddTokenBlackList(this IServiceCollection collection)
    {
        collection.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();
        collection.AddMemoryCache();
    }
}