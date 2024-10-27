using Application.DTOs;
using Application.Interfaces;
using Application.Util;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class UCRegister
{
    private readonly ILogger<UCRegister> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IIdentRepository _identRepository;
    private readonly PasswordUtil _passwordUtil;

    public UCRegister(ILogger<UCRegister> logger, IUserRepository userRepository, IIdentRepository identRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _identRepository = identRepository;
        _passwordUtil = new PasswordUtil();
    }

    public async Task<(bool Success, string Message)> Register(RegisterDTO registerDTO)
    {
        _logger.LogDebug("UCRegister - Register");
        _logger.LogDebug(registerDTO.ToString());
        if (!registerDTO.agb) return (false, "AGB nicht akzeptiert");
        if (await _identRepository.FindByAsync(x => x.IdentId == registerDTO.indentId) != null)
            return (false, "Ident doppelt");
        if (await _userRepository.FindByAsync(x => x.Username == registerDTO.username) != null)
            return (false, "Benutzername vergeben");

        var user = new User(registerDTO.username,
            _passwordUtil.HashPassword(registerDTO.username, registerDTO.password));
        if (!_userRepository.SelectAsync().Result.Any()) user.Role = Role.Operator;
        _userRepository.Add(user);
        await _userRepository.SaveChangesAsync();
        var ident = new Ident(registerDTO.indentId);
        _identRepository.Add(ident);
        await _identRepository.SaveChangesAsync();

        return (true, "User registriert");
    }
}