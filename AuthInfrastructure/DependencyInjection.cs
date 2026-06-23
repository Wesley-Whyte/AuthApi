using AuthApplication.Interface;
using AuthInfrastructure.Data;
using AuthInfrastructure.Repositories;
using AuthInfrastructure.Services;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthInfrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        // register credential built from app registration
        services.AddSingleton<TokenCredential>(_ => new ClientSecretCredential(configuration["Azure:TenantId"], configuration["Azure:ClientId"], configuration["Azure:ClientSecret"]));

        // register interceptor
        services.AddSingleton<AzureAdTokenConnectionInterceptor>();

        // Register your DbContext and repositories here
        services.AddDbContext<ApplicationDbContext>((provider, options) =>
            options.UseSqlServer(configuration.GetConnectionString("AzureConnection"), options => { })
            .AddInterceptors(provider.GetRequiredService<AzureAdTokenConnectionInterceptor>()));

        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 12;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+!#$%^&*() ";
        }).AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }

}
