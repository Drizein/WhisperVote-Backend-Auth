namespace Application.Util;

public interface IPasswordUtil
{
    string HashPassword(string username, string password);
    bool VerifyPassword(string password, string hash, string username);
}