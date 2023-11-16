using Jwt.Application.Model;
using Jwt.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Jwt.Application.Interfaces;

public interface IUserManager
{
    Task<User> RegisterUser(UserDto request);
    Task<string?> LoginUser(UserDto request, HttpContext httpContext);
    Task<string?> GenerateRefreshToken(HttpContext httpContext);
    Task<string?> SetUserRoleAsync(RoleDto dto);
}
