using System;
using AuthApplication.Dtos.Account;
using AuthApplication.Interface;
using AuthInfrastructure.Services;
using Microsoft.AspNetCore.Identity;

namespace AuthInfrastructure.Repositories;

public class UserRepository : IUserRepository
{
    protected readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenService _tokenService;
    public UserRepository(UserManager<IdentityUser> userManager, ITokenService tokenService)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }
    public async Task<NewUserDto> CreateUserAsync(RegisterDto registerDto)
    {
        var user = new IdentityUser
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
            EmailConfirmed = true
        };

        var newUser = await _userManager.CreateAsync(user, registerDto.Password);
        if (!newUser.Succeeded) throw new Exception("User creation failed. " + newUser.Errors.FirstOrDefault()?.Description);

        var role = await _userManager.AddToRoleAsync(user, "User");
        if (!role.Succeeded) throw new Exception("Failed to assign role to user" + role.Errors.FirstOrDefault()?.Description);

        var roles = await _userManager.GetRolesAsync(user);
        return new NewUserDto
        {
            Email = user.Email,
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user, roles)
        };
    }

    public async Task<NewUserDto> LoginAsync(LoginDto loginDto)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.UserName == loginDto.Username);
        if (user == null || user.Email == null || user.UserName == null) throw new Exception("User not found");
        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result) throw new Exception("Invalid password");
        var roles = await _userManager.GetRolesAsync(user);
        return new NewUserDto
        {
            Email = user.Email,
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user, roles)
        };
    }
}
