using Application.DTOs;
using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<RegisterController> _logger;
    private readonly UCLogin _ucLogin;

    public LoginController(ILogger<RegisterController> logger, UCLogin ucLogin)
    {
        _logger = logger;
        _ucLogin = ucLogin;
    }

    [HttpPost]
    public async Task<ActionResult<string>> Login([FromBody] LoginDTO loginDTO)
    {
        _logger.LogDebug("loginDTO - Login");
        var (Success, Message) = await _ucLogin.Login(loginDTO);
        if (Success) return Ok(Message); // HTTP 200
        return BadRequest(Message); // HTTP 400
    }
}