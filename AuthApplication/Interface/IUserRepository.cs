using System;
using AuthApplication.Dtos.Account;

namespace AuthApplication.Interface;

public interface IUserRepository
{
    Task<NewUserDto> CreateUserAsync(RegisterDto registerDto);
    Task<NewUserDto> LoginAsync(LoginDto loginDto);
    // Other user-related methods can be defined here
}
