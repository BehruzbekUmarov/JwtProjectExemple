using Jwt.Application.Model;
using Jwt.Domain.Entities;

namespace Jwt.Application.Interfaces;

public interface ITokenManager
{
    public string GenerateToken(User user);
    public RefreshToken GenerateRefreshToken();
}
