using Jwt.Application.Model;
using Jwt.WebUI.Entities;
using Jwt.WebUI.Model;

namespace Jwt.WebUI.Managers.Interfaces
{
    public interface IUserManager
    {
        Task<User> RegisterUser(UserDto request);
        Task<string?> LoginUser(UserDto request, HttpContent httpContext);
        Task<string?> GenerateRefreshToken(HttpContent httpContext);
        Task<string?> SetUserRoleAsync(SetRoleDto dto);
    }
}
