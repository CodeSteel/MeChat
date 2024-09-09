using MeChat.Contexts;
using MeChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeChat.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private ApplicationDataContext _dataContext;
    
    public AccountController(ILogger<AccountController> logger, ApplicationDataContext dataContext)
    {
        _logger = logger;
        _dataContext = dataContext;
    }
    
    [Authorize]
    public async Task<IActionResult> Index()
    {
        User? user = await _dataContext.Users
            .Include(x => x.Friends)
            .Include(x => x.FriendRequests)
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
        return View(user);
    }
    
    [Authorize]
    public async Task<IActionResult> Profile(Guid userId)
    {
        User? user = await _dataContext.Users
            .Include(x => x.Friends)
            .Include(x => x.ChatGroups)
            .FirstOrDefaultAsync(x => x.Id == userId);
        return View(user);
    }
    
    [Authorize]
    public async Task<IActionResult> SaveAccount(User userForm)
    {
        User? user = await _dataContext.Users
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        _dataContext.Update(user);
        user.DisplayName = userForm.DisplayName;
        await _dataContext.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }
}