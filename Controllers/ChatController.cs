using System.Security.Claims;
using System.Text.Json;
using MeChat.Contexts;
using MeChat.Models;
using MeChat.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeChat.Controllers;

[Route("api/[controller]")]
[Authorize]
public class ChatController : Controller
{
    private readonly ApplicationDataContext _dataContext;
    private readonly UserManager<User> _userManager;
    
    public ChatController(ApplicationDataContext dataContext, UserManager<User> userManager)
    {
        _dataContext = dataContext;
        _userManager = userManager;
    }
    
    [HttpGet("group/getMessages")]
    [Authorize]
    public async Task<IActionResult> GetChatGroupMessages(Guid groupId)
    {
        ChatGroup? group = await _dataContext.ChatGroups.Include(x => x.Chats).ThenInclude(x => x.PostedBy).FirstOrDefaultAsync(x => x.Id == groupId);
        if (group == null)
        {
            return BadRequest($"Couldn't find ChatGroup with Id: '{groupId}'.");
        }

        return Json(group.Chats.ToList());
    }
    
    [HttpPost("group/sendMessage")]
    [Authorize]
    public async Task<IActionResult> CreateMessage(CreateChatRequest request)
    {
        if (User.Identity == null)
        {
            return BadRequest($"User missing identity.");
        }

        User? requestUser = await _userManager.GetUserAsync(new ClaimsPrincipal(User.Identity));
        if (requestUser == null)
        {
            return BadRequest($"Failed to get request user.");
        }
        
        ChatGroup? group = await _dataContext.ChatGroups.FindAsync(request.GroupId);
        if (group == null)
        {
            return BadRequest($"Couldn't find ChatGroup with Id: '{request.GroupId}'.");
        }

        Chat newChat = new Chat()
        {
            Id = Guid.NewGuid(),
            Body = request.Body,
            CreatedAt = DateTime.Now.ToUniversalTime(),
            Group = group,
            PostedBy = requestUser
        };

        await _dataContext.Chats.AddAsync(newChat);
        await _dataContext.SaveChangesAsync();

        return Redirect("/");
    }
    
    [HttpPost("group/create")]
    [Authorize]
    public async Task<IActionResult> CreateGroup(CreateGroupRequest request)
    {
        if (User.Identity == null)
        {
            return BadRequest($"User missing identity.");
        }

        User? requestUser = await _userManager.GetUserAsync(new ClaimsPrincipal(User.Identity));
        if (requestUser == null)
        {
            return BadRequest($"Failed to get request user.");
        }
        
        ChatGroup newChatGroup = new ChatGroup()
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };
        newChatGroup.Users.Add(requestUser);

        await _dataContext.ChatGroups.AddAsync(newChatGroup);
        await _dataContext.SaveChangesAsync();
        
        return Redirect("/");
    }
}