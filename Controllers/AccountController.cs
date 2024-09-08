using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeChat.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    
    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        return Ok();
    }
}