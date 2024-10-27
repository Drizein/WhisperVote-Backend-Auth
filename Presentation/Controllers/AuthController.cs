using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult> ValidateToken([FromHeader] string Authorization)
    {
        return Ok();
    }
}