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
    
    public async Task<IActionResult> Index()
    {
        AppStatistic? appStatistic = await _dataContext.AppStatistics.FirstOrDefaultAsync();
        return View(appStatistic);
    }

    public async Task<IActionResult> DirectMessage(Guid? userId)
    {
        if (userId == null)
        {
            return BadRequest();
        }
        
        User? user = await _dataContext.Users
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
        
        if (user == null)
        {
            return BadRequest();
        }
        
        User? targetUser = await _dataContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId);
        
        if (targetUser == null)
        {
            return BadRequest();
        }

        ChatGroup? chatGroup = await _dataContext.ChatGroups
            .FirstOrDefaultAsync(x =>
                x.Type == ChatGroupType.DirectMessage && x.Users.Contains(user) && x.Users.Contains(targetUser));

        Guid id;
        
        if (chatGroup == null)
        {
            ChatGroup groupChat = new ChatGroup();

            id = groupChat.Id;
            
            groupChat.Users.Add(user);
            groupChat.Users.Add(targetUser);
            await _dataContext.ChatGroups.AddAsync(groupChat);
            await _dataContext.SaveChangesAsync();
        }
        else
        {
            id = chatGroup.Id;
        }
        
        return RedirectToAction("Dashboard", "Home", new { groupId= id });
    }

    [Authorize]
    public async Task<IActionResult> Dashboard(Guid? groupId)
    {
        User? user = await _dataContext.Users
            .Include(x => x.ChatGroups)
            .ThenInclude(x => x.Users)
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
        
        if (user == null)
        {
            return View();
        }

        List<ChatGroup> groupsWithUser = user.ChatGroups.ToList();
        ChatGroup? selectedGroup = null;

        if (groupsWithUser.Count > 0)
            selectedGroup = groupsWithUser[0];
        
        if (groupId != null)
        {
            ChatGroup? group = groupsWithUser.FirstOrDefault(x => x.Id == groupId);
            if (group != null)
                selectedGroup = group;
        }

        if (selectedGroup != null)
        {
            selectedGroup = _dataContext.ChatGroups
                .Include(x => x.Chats)
                .ThenInclude(x => x.PostedBy)
                .ThenInclude(x => x.ProfilePicture)
                .FirstOrDefault(x => x.Id == selectedGroup.Id);
        }

        DashboardResults newIndex = new DashboardResults()
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
            publicGroups = _dataContext?.ChatGroups.Include(x => x.Users).Where(x => x.Type == ChatGroupType.PublicGroup && x.Name.ToLower().Contains(groupCollection.Search.ToLower())).ToList() ?? new List<ChatGroup>();
        }
        else
        {
            publicGroups = _dataContext?.ChatGroups.Include(x => x.Users).Where(x => x.Type == ChatGroupType.PublicGroup).ToList() ?? new List<ChatGroup>();
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