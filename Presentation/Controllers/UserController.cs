using Application.DTOs;
using Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UCUserRole _ucUserRole;

    public UserController(ILogger<UserController> logger, UCUserRole ucUserRole)
    {
        _logger = logger;
        _ucUserRole = ucUserRole;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> ChangeRoleForUser([FromHeader] string Authorization,
        [FromBody] ChangeRoleDTO changeRoleDTO)
    {
        _logger.LogDebug("changeRole - Change role for user");
        var (Success, Message) = await _ucUserRole.ChangeRole(Authorization, changeRoleDTO);
        if (Success) return Ok(Message); // HTTP 200
        return BadRequest(Message); // HTTP 400
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult> GetRoleForUser([FromHeader] string Authorization)
    {
        _logger.LogDebug("GetRole - Get role for user");
        var (Success, Message) = await _ucUserRole.GetRole(Authorization);
        if (Success) return Ok(Message); // HTTP 200
        return BadRequest(Message); // HTTP 400
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult> GetTeamMembers([FromHeader] string Authorization)
    {
        _logger.LogDebug("GetTeamMembers - Get team members");
        var (Success, Message) = await _ucUserRole.GetTeamMembers(Authorization);
        if (Success) return Ok(Message); // HTTP 200
        return BadRequest(Message); // HTTP 400
    }

    [Authorize]
    [HttpPatch]
    public async Task<ActionResult> StrikeUser([FromHeader] string Authorization, [FromQuery] Guid strikedUserId)
    {
        _logger.LogDebug("StrikeUser - Strike user");
        _logger.LogDebug(strikedUserId.ToString());
        var (Success, Message) = await _ucUserRole.StrikeUser(Authorization, strikedUserId);
        if (Success) return Ok(Message); // HTTP 200
        return BadRequest(Message); // HTTP 400
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult> IsUserStruck([FromHeader] string Authorization)
    {
        _logger.LogDebug("GetStrikes - Get strikes for user");
        var (Success, Message) = await _ucUserRole.IsUserStruck(Authorization);
        if (Success) return Ok(Message); // HTTP 200
        return BadRequest(Message); // HTTP 400
    }
}