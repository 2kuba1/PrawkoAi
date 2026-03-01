using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Contracts.Services;
using Domain;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Persistence.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string CreateToken(User user)
    {
        var secretKey = _configuration["Jwt:Secret"];
        if(secretKey is null)
            throw new InvalidOperationException("Missing configuration secret");
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
            SigningCredentials = credentials,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
        };
        
        var claimsIdentity = new ClaimsIdentity([
            new Claim("id", user.Id.ToString()),
            new Claim("device_id", user.DeviceId),
            new Claim("role", user.Role?.Name ?? "User")
        ]);
        
        if (user.Email is not null)
            claimsIdentity.AddClaim(new Claim("email", user.Email));

        tokenDescriptor.Subject = claimsIdentity;

        var handler = new JsonWebTokenHandler();

        var token = handler.CreateToken(tokenDescriptor);
        
        return token;
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}