using Jwt.Application.Interfaces;
using Jwt.Application.Model;
using Jwt.Application.Options;
using Jwt.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Jwt.Infrastucture.Services;

public class TokenManager : ITokenManager
{
    private readonly IOptions<JwtOptions> _options;
    public TokenManager(IOptions<JwtOptions>? options)
    {
        _options = options;
    }
    public RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddDays(4),
            Created = DateTime.Now
        };

        return refreshToken;
    }

    public string GenerateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new ("Id", user.Id.ToString()),
            new (JwtRegisteredClaimNames.Name, user.UserName)
        };

        claims.Add(new("Role", user.Role.ToString()));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.Key));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(2),
            audience: _options.Value.Audience,
            issuer: _options.Value.Issuer,
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
