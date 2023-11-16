using Jwt.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Jwt.Infrastucture.Services;

public class PasswordManager : IPasswordManager
{
    public Task HashPassword(string password, out byte[] hashedPassword, out byte[] hashedSalt)
    {
        using var hmac = new HMACSHA512();
        hashedSalt = hmac.Key;
        hashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return Task.CompletedTask;
    }

    public Task<bool> VerifyHashedPassword(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Task.FromResult(computedHash.SequenceEqual(passwordHash));
    }
}
