using Domain.Entities;

namespace Application.Util;

public interface IJwtUtil
{
    string GenerateToken(User user);
    string ParseJwt(string token);
}