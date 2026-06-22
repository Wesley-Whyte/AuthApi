using AuthApplication.Dtos.Account;
using AuthApplication.Interface;
using AuthApplication.Services;
using AuthInfrastructure;
using DotNetEnv;
using Microsoft.AspNetCore.Http.HttpResults;

internal class Program
{
    private static void Main(string[] args)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            Env.Load();

        var builder = WebApplication.CreateBuilder(args);
        var authApiBaseUrl = builder.Configuration["AuthApi:BaseUrl"];
        var JWTSigninKey = builder.Configuration["JWT:SigninKey"];
        Console.WriteLine("Starting Auth API... Base URL: " + authApiBaseUrl + " JWT Signin Key: " + JWTSigninKey);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddAuthInfrastructure(builder.Configuration);
        builder.Services.AddScoped<IAccountService, AccountService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.MapPost("account/register", async (RegisterDto registerDto, IAccountService _accountService) =>
        {
            try
            {
                var newUser = await _accountService.CreateUserAsync(registerDto);
                if (newUser == null) return Results.BadRequest("User creation failed at Controller level");
                return Results.Ok(newUser);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });

        app.MapPost("account/login", async (LoginDto loginDto, IAccountService accountService) =>
        {
            try
            {
                var user = await accountService.LoginAsync(loginDto);
                if (user == null) return Results.BadRequest("Login Failed at Controller level");
                return Results.Ok(user);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });

        app.Run();
    }
}