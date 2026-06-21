using System;
using System.Runtime.CompilerServices;
using AuthApplication.Dtos.Account;

namespace AuthApplication.Interface;

public interface IAccountService
{
    public Task<NewUserDto> CreateUserAsync(RegisterDto registerDto);
    public Task<NewUserDto> LoginAsync(LoginDto loginDto);
}
