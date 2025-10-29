using System.Text;
using CapstoneBackend.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CapstoneBackend.Auth;

public static class AuthSetup
{
    public static IServiceCollection AddAuth(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ClaimUtility>(new ClaimUtility(configuration));
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IAuthServiceWrapper, AuthServiceWrapper>();
        services.AddScoped<IAuthService, AuthService>();
        
        
        services.AddAuthentication(configuration);
        services.AddAuthorization();
        
        return services;
    }
    
    private static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var key = Encoding.ASCII.GetBytes(configuration[EnvironmentVariables.TOKEN_KEY]!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Events = SetupBearerEvents();
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    }

    private static JwtBearerEvents SetupBearerEvents()
    {
        return new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                return Task.CompletedTask;
            }
            //TODO add more bearer events for better error messaging
        };
    }
}