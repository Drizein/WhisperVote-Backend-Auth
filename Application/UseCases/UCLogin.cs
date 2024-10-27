using Application.DTOs;
using Application.Interfaces;
using Application.Util;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class UCLogin
{
    private readonly ILogger<UCRegister> _logger;
    private readonly IUserRepository _userRepository;
    private readonly PasswordUtil _passwordUtil;
    private readonly JwtUtil _jwtUtil;

    public UCLogin(ILogger<UCRegister> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _passwordUtil = new PasswordUtil();
        _jwtUtil = new JwtUtil();
    }

    public async Task<(bool Success, string Message)> Login(LoginDTO loginDTO)
    {
        _logger.LogDebug("UCLogin - Login");
        _logger.LogDebug(loginDTO.ToString());

        var user = await _userRepository.FindByAsync(x => x.Username == loginDTO.username);

        if (user == null || !_passwordUtil.VerifyPassword(loginDTO.password, user.Password, user.Username))
            return (false, "Benutzername oder Passwort falsch");

        return (true, _jwtUtil.GenerateToken(user));
    }
}