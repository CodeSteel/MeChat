using System.Diagnostics;
using MeChat.Contexts;
using Microsoft.AspNetCore.Mvc;
using MeChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
    
    public async Task<IActionResult> Index(Guid? groupId)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }
        
        User? user = await _dataContext.Users.Include(x => x.ChatGroups).ThenInclude(x => x.Users).FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
        if (user == null)
        {
            return View();
        }

        List<ChatGroup> groupsWithUser = user.ChatGroups.ToList();
        ChatGroup selectedGroup = groupsWithUser[0];
        
        if (groupId != null)
        {
            ChatGroup? group = groupsWithUser.FirstOrDefault(x => x.Id == groupId);
            if (group != null)
                selectedGroup = group;
        }
        
        await _dataContext.Entry(selectedGroup)
            .Collection(b => b.Chats)
            .LoadAsync();

        IndexResult newIndex = new IndexResult()
        {
            Selection = groupId ?? Guid.Empty,
            GroupsWithUser = groupsWithUser,
            SelectedGroup = selectedGroup
        };
        return View(newIndex);
    }

    [Authorize]
    public IActionResult CreateGroup()
    {
        return View();
    }

    [Authorize]
    public IActionResult FindGroup(FindGroupResult? groupCollection)
    {
        List<ChatGroup> publicGroups;
        if (groupCollection != null && !string.IsNullOrEmpty(groupCollection.Search))
        {
            publicGroups = _dataContext?.ChatGroups.Include(x => x.Users).Where(x => x.Public && x.Name.ToLower().Contains(groupCollection.Search.ToLower())).ToList() ?? new List<ChatGroup>();
        }
        else
        {
            publicGroups = _dataContext?.ChatGroups.Include(x => x.Users).Where(x => x.Public).ToList() ?? new List<ChatGroup>();
        }
        
        return View(new FindGroupResult()
        {
            ChatGroups = publicGroups
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}