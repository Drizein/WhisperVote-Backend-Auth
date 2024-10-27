using Microsoft.AspNetCore.Identity;

namespace Application.Util;

public class PasswordUtil : IPasswordUtil
{
    private readonly PasswordHasher<string> _ph = new();

    public string HashPassword(string username, string password)
    {
        return _ph.HashPassword(username, password);
    }

    public bool VerifyPassword(string password, string hash, string username)
    {
        return _ph.VerifyHashedPassword(username, hash, password) == PasswordVerificationResult.Success;
    }
}