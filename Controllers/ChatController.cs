using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeChat.Controllers;

[Route("api/[controller]")]
[Authorize]
public class ChatController : Controller
{
    [HttpGet("test")]
    [Authorize]
    public IActionResult Test()
    {
        return Ok("Good");
    }
}