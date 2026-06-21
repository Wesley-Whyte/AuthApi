using System;
using Microsoft.AspNetCore.Identity;

namespace AuthInfrastructure.Services;

public interface ITokenService
{
    string CreateToken(IdentityUser user, IList<string> roles);
    /*Task<string> CreateRefreshTokenAsync(IdentityUser user);
    Task<bool> ValidateTokenAsync(string token);
    Task<IdentityUser> GetUserFromTokenAsync(string token);*/
}
