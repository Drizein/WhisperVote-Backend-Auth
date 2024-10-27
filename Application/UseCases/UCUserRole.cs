using System.Text.Json;
using Application.DTOs;
using Application.Interfaces;
using Application.Util;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class UCUserRole
{
    private readonly ILogger<UCUserRole> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IJwtUtil _jwtUtil;

    private static readonly int AllowedStrikes = int.Parse(Environment.GetEnvironmentVariable("AllowedStrikes"));

    public UCUserRole(ILogger<UCUserRole> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _jwtUtil = new JwtUtil();
    }

    public UCUserRole(ILogger<UCUserRole> logger, IUserRepository userRepository, IJwtUtil jwtUtil)
    {
        _logger = logger;
        _userRepository = userRepository;
        _jwtUtil = jwtUtil;
    }

    public async Task<(bool Success, string Message)> ChangeRole(string authorization, ChangeRoleDTO changeRoleDto)
    {
        _logger.LogDebug("UCChangeRole - ChangeRole");
        var (userRole, valueTuple) = await GetUserRole(authorization);
        if (userRole == null) return valueTuple;

        User userToChangeRole; // zuändernden User
        try
        {
            userToChangeRole = await _userRepository.FindByAsync(x => x.Id == Guid.Parse(changeRoleDto.userId));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error parsing user guid");
            return (false, "Fehler beim parsen der GUID");
        }

        if (userToChangeRole == null) return (false, "Zuändernden User nicht gefunden");

        switch (userRole)
        {
            case Role.Operator:
                if (changeRoleDto.role is not Role.Operator)
                {
                    userToChangeRole.Role = changeRoleDto.role;
                    await _userRepository.SaveChangesAsync();
                    return (true, "Rolle geändert");
                }

                break;
            case Role.Admin:
                if (changeRoleDto.role is Role.Moderator or Role.User)
                {
                    userToChangeRole.Role = changeRoleDto.role;
                    await _userRepository.SaveChangesAsync();
                    return (true, "Rolle geändert");
                }

                break;
            default:
                return (false, "Nicht berechtigt");
        }

        return (false, "Unbekannter Fehler");
    }

    public async Task<(bool Success, string? Message)> GetRole(string authorization)
    {
        var (userRole, valueTuple) = await GetUserRole(authorization);
        return userRole == null ? valueTuple : (true, userRole.ToString());
    }

    public async Task<(bool Success, string? Message)> GetTeamMembers(string authorization)
    {
        var (userRole, valueTuple) = await GetUserRole(authorization);
        if (userRole == null) return valueTuple;
        List<UserDTO> userDtos = [];

        switch (userRole)
        {
            case Role.Operator:
                foreach (var user in await _userRepository.FilterAsync(x =>
                             x.Role == Role.Admin || x.Role == Role.Moderator))
                    userDtos.Add(new UserDTO(user.Id, user.Role));
                return (true, JsonSerializer.Serialize(userDtos));
            case Role.Admin:
                foreach (var user in await _userRepository.FilterAsync(x => x.Role == Role.Moderator))
                    userDtos.Add(new UserDTO(user.Id, user.Role));
                return (true, JsonSerializer.Serialize(userDtos));
            default:
                return (false, "Nicht berechtigt");
        }
    }

    public async Task<(bool Success, string? Message)> StrikeUser(string authorization, Guid strikedUserId)
    {
        var (userRole, valueTuple) = await GetUserRole(authorization);
        if (userRole == null) return valueTuple;
        if (userRole == Role.User) return (false, "Nicht berechtigt");
        var userToStrike = await _userRepository.FindByAsync(x => x.Id == strikedUserId);
        if (userToStrike == null) return (false, "Benutzer nicht gefunden");
        userToStrike.Strikes++;
        await _userRepository.SaveChangesAsync();
        return (true, "Benutzer gestriked");
    }

    public async Task<(bool Success, string? Message)> IsUserStruck(string authorization)
    {
        string userId;
        try
        {
            userId = _jwtUtil.ParseJwt(authorization);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error parsing jwt");
            return (false, "Fehler beim parsen des JWT");
        }

        if (_userRepository.FindByAsync(x => x.Id == Guid.Parse(userId)).Result.Strikes > AllowedStrikes)
            return (true, "true");
        return (true, "false");
    }

    private async Task<(Role? userRole, (bool Success, string Message) valueTuple)> GetUserRole(string authorization)
    {
        _logger.LogDebug("UCGetRole - Get role");
        User user; // anfragenden User
        string userId;
        try
        {
            userId = _jwtUtil.ParseJwt(authorization);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error parsing jwt");
            return (null, (false, "Fehler beim parsen des JWT"));
        }

        try
        {
            user = await _userRepository.FindByAsync(x => x.Id == Guid.Parse(userId));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error parsing user guid");
            return (null, (false, "Fehler beim parsen der GUID"));
        }

        if (user == null) return (null, (false, "Anfragenden User nicht gefunden"));
        return (user.Role, (true, "Rolle gefunden"));
    }
}