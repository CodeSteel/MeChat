using System.Diagnostics;
using System.Security.Claims;
using MeChat.Contexts;
using Microsoft.AspNetCore.Mvc;
using MeChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace MeChat.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDataContext _dataContext;
    private readonly UserManager<User> _userManager;
    
    public HomeController(ILogger<HomeController> logger, ApplicationDataContext dataContext, UserManager<User> userManager)
    {
        _logger = logger;
        _dataContext = dataContext;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }
        return View();
    }

    [Authorize]
    public IActionResult CreateGroup()
    {
        return View();
    }

    [Authorize]
    public IActionResult FindGroup()
    {
        return View();
    }
    
    [Authorize]
    public async Task<IActionResult> LeaveGroup(Guid groupId)
    {
        User? user = await _userManager.GetUserAsync(new ClaimsPrincipal(User.Identity));
        if (user != null)
        {
            _logger.LogInformation($"GroupId={groupId}");
            ChatGroup? requestedGroup = await _dataContext.ChatGroups.Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.Id == groupId);
            if (requestedGroup != null)
            {
                _logger.LogInformation($"UserCount: {requestedGroup.Users.Count}");
                User? foundUser = requestedGroup.Users.FirstOrDefault(x => x.Id == user.Id);
                if (foundUser != null)
                {
                    _dataContext.ChatGroups.Update(requestedGroup);
                    requestedGroup.Users.Remove(user);
                    await _dataContext.SaveChangesAsync();
                }
            }
            else
            {
                _logger.LogInformation($"GROUP IS NULL");
            }
        }
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}