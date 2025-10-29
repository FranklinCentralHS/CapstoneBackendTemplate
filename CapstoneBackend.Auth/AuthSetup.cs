using Microsoft.Extensions.DependencyInjection;

namespace CapstoneBackend.Auth;

public static class AuthSetup
{
    public static IServiceCollection AddAuth(IServiceCollection services)
    {
        services.AddSingleton<ClaimUtility>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IAuthServiceWrapper, AuthServiceWrapper>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}