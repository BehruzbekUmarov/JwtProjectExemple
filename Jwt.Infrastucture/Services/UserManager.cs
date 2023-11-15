using Jwt.Application.Interfaces;
using Jwt.Application.Model;
using Jwt.WebUI.Data;
using Jwt.WebUI.Entities;
using Jwt.WebUI.Managers.Interfaces;
using Jwt.WebUI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Jwt.Infrastucture.Services
{
    public class UserManager : IUserManager
    {
        private readonly IPasswordManager _passwordManager;
        private readonly ITokenManager _tokenManager;
        private readonly IRoleManager _roleManager;
        private readonly ApplicationDbContext _context;
        public UserManager(IPasswordManager? passwordManager,
            ITokenManager? tokenManager,
            IRoleManager? roleManager,
            ApplicationDbContext context)
        {
            _passwordManager = passwordManager;
            _tokenManager = tokenManager;
            _roleManager = roleManager;
            _context = context;
        }
        public Task<string?> GenerateRefreshToken(HttpContent httpContext)
        {
            var refreshToken = httpContext.Request.Cookies["refreshToken"];
            if (refreshToken is null)
            {
                throw new UnauthorizedAccessException("Cookies not Found");
            }

            if (!int.TryParse(httpContext.User.FindFirstValue("Id"), out var id))
                throw new UnauthorizedAccessException("User Id not found");
        }

        public async Task<string?> LoginUser(UserDto request, HttpContent httpContext)
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

        public Task<string?> SetUserRoleAsync(SetRoleDto dto)
        {
            throw new NotImplementedException();
        }

        private async Task SetRefreshToken(RefreshToken newRefreshToken,
            HttpContent httpContext, User user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires,
            };

            httpContext.Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpired = newRefreshToken.Expires;

            await _dbContext.SaveChangesAsync();
        }
    }
}
