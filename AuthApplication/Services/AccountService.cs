using System;
using AuthApplication.Dtos.Account;
using AuthApplication.Interface;

namespace AuthApplication.Services;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    public AccountService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<NewUserDto> CreateUserAsync(RegisterDto registerDto)
    {
        var newUser = await _userRepository.CreateUserAsync(registerDto);
        if (newUser == null) throw new Exception("User creation failed at Account Service level");
        return newUser;
    }

    public async Task<NewUserDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.LoginAsync(loginDto);
        if (user == null) throw new Exception("Login Failed at Account Service level");
        return user;
    }
}