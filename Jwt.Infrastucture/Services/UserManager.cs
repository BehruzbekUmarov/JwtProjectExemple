using Jwt.Application.Interfaces;
using Jwt.Application.Model;
using Jwt.Domain.Data;
using Jwt.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Jwt.Infrastucture.Services;

public class UserManager : IUserManager
{
    private readonly IPasswordManager _passwordManager;
    private readonly ITokenManager _tokenManager;
    private readonly ApplicationDbContext _context;
    public UserManager(IPasswordManager? passwordManager,
        ITokenManager? tokenManager,
        ApplicationDbContext context)
    {
        _passwordManager = passwordManager;
        _tokenManager = tokenManager;
        _context = context;
    }
    public async Task<string?> GenerateRefreshToken(HttpContext httpContext)
    {
        var refreshToken = httpContext.Request.Cookies["refreshToken"];
        if (refreshToken is null)

        {
            throw new UnauthorizedAccessException("Cookies not Found");
        }

        if (!int.TryParse(httpContext.User.FindFirstValue("Id"), out var id))
            throw new UnauthorizedAccessException("User Id not found");

        var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new UserNotFoundException("User not exist");

        if (!user.RefreshToken!.Equals(refreshToken))
            throw new UnauthorizedAccessException("RefreshToken did not match!!");

        if (user.TokenExpires < DateTime.Now)
            throw new WrongInputException("Token not expired");

        string jwtToken = _tokenManager.GenerateToken(user);
        var newRefreshToken = _tokenManager.GenerateRefreshToken();

        await SetRefreshToken(newRefreshToken, httpContext, user);

        return jwtToken;
    }

    public async Task<string?> LoginUser(UserDto request, HttpContext httpContext)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == x.UserName)
            ?? throw new Exception("User Not Found");

        if (!await _passwordManager.VerifyHashedPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            throw new Exception("Password or Name is Wrong!!");

        string jwtToken = _tokenManager.GenerateToken(user);

        var newRefreshToken = _tokenManager.GenerateRefreshToken();

        await SetRefreshToken(newRefreshToken, httpContext, user);

        return jwtToken;
    }

    public async Task<User> RegisterUser(UserDto request)
    {
        await _passwordManager.HashPassword(request.Password,
            out byte[] passwordHash,
            out byte[] passwordSalt);

        var user = new User
        {
            UserName = request.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<string?> SetUserRoleAsync(RoleDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId)
        ?? throw new UserNotFoundException("User not found.");

        user.Role = dto.Role;
        await _context.SaveChangesAsync();

        var newJwt = _tokenManager.GenerateToken(user);
        return newJwt;
    }

    private async Task SetRefreshToken(RefreshToken newRefreshToken,
        HttpContext httpContext, User user)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = newRefreshToken.Expires,
        };

        httpContext.Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
        user.RefreshToken = newRefreshToken.Token;
        user.TokenCreated = newRefreshToken.Created;
        user.TokenExpires = newRefreshToken.Expires;

        await _context.SaveChangesAsync();
    }
}
