using AuthApplication.Interface;
using AuthInfrastructure.Data;
using AuthInfrastructure.Repositories;
using AuthInfrastructure.Services;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthInfrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration, bool IsDevelopment = false)
    {
        var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            ExcludeManagedIdentityCredential = IsDevelopment,
            ExcludeWorkloadIdentityCredential = IsDevelopment,
        });
        services.AddSingleton<TokenCredential>(credentials);
        // Register your DbContext and repositories here
        services.AddDbContext<ApplicationDbContext>((provider, options) =>
        {
            var credential = provider.GetRequiredService<TokenCredential>();
            var sqlConnection = new SqlConnection(configuration.GetConnectionString("AzureConnection"))
            {
                AccessToken = credential
                    .GetToken(
                        new TokenRequestContext(new[] { "https://database.windows.net/.default" }),
                        default)
                    .Token
            };
            options.UseSqlServer(sqlConnection);
        });

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
