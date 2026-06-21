using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthInfrastructure.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["JWT:SigninKey"] ?? throw new ArgumentNullException("TokenKey is not configured")));

    }
    public string CreateToken(IdentityUser user, IList<string> roles)
    {
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? throw new ArgumentNullException("Email is null")),
            new Claim(JwtRegisteredClaimNames.GivenName, user.UserName ?? throw new ArgumentNullException("UserName is null"))
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        return jwtToken;
    }
}
