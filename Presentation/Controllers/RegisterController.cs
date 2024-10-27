using Application.DTOs;
using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class RegisterController : ControllerBase
{
    private readonly ILogger<RegisterController> _logger;
    private readonly UCRegister _ucRegister;

    public RegisterController(ILogger<RegisterController> logger, UCRegister ucRegister)
    {
        _logger = logger;
        _ucRegister = ucRegister;
    }

    [HttpPost]
    public async Task<ActionResult<string>> Register([FromBody] RegisterDTO registerDTO)
    {
        _logger.LogDebug("RegisterDTO - Register");
        var (Success, Message) = await _ucRegister.Register(registerDTO);
        if (Success) return Ok(Message); // HTTP 200
        return BadRequest(Message); // HTTP 400
    }
}