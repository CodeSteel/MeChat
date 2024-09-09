using System.Security.Claims;
using MeChat.Contexts;
using MeChat.Models;
using MeChat.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeChat.Controllers;

[Route("api")]
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

    [HttpPost("group/message")]
    [Authorize]
    public async Task<IActionResult> SendMessage(CreateChatRequest request)
    {
        if (!request.IsValid)
        {
            return BadRequest($"Request is invalid.");
        }
        
        if (User.Identity == null)
        {
            return BadRequest($"User missing identity.");
        }

        User? requestUser = await _userManager.GetUserAsync(new ClaimsPrincipal(User.Identity));
        if (requestUser == null)
        {
            return BadRequest($"Failed to get request user.");
        }
        
        ChatGroup? group = await _dataContext.ChatGroups.Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == request.GroupId);
        if (group == null)
        {
            return BadRequest($"Couldn't find ChatGroup with Id: '{request.GroupId}'.");
        }

        if (group.Users.FirstOrDefault(x => x.Id == requestUser.Id) == null)
        {
            return BadRequest($"User is not in group.");
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

        return Ok();
    }
    
    [HttpPost("group/create")]
    [Authorize]
    public async Task<IActionResult> CreateGroup(CreateGroupRequest request)
    {
        if (!request.IsValid)
        {
            return BadRequest($"Request is invalid.");
        }
        
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
            Name = request.Name,
            Type = ChatGroupType.PublicGroup,
            Owner = requestUser
        };
        newChatGroup.Users.Add(requestUser);

        await _dataContext.ChatGroups.AddAsync(newChatGroup);
        await _dataContext.SaveChangesAsync();
        
        return RedirectToAction("Index", "Home", new {GroupId = newChatGroup.Id});
    }
    
    [HttpPost("group/leave")]
    [Authorize]
    public async Task<IActionResult> LeaveGroup(Guid groupId)
    {
        User? user = await _userManager.GetUserAsync(new ClaimsPrincipal(User.Identity));
        if (user != null)
        {
            ChatGroup? requestedGroup = await _dataContext.ChatGroups.Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.Id == groupId);
            if (requestedGroup != null)
            {
                User? foundUser = requestedGroup.Users.FirstOrDefault(x => x.Id == user.Id);
                if (foundUser != null)
                {
                    if (requestedGroup.Users.Count > 1)
                    {
                        _dataContext.ChatGroups.Update(requestedGroup);
                        requestedGroup.Users.Remove(user);
                    }
                    else
                    {
                        _dataContext.ChatGroups.Remove(requestedGroup);
                    }
                    
                    await _dataContext.SaveChangesAsync();
                }
            }
        }
        return RedirectToAction("Index", "Home");
    }
    
    [HttpPost("group/join")]
    [Authorize]
    public async Task<IActionResult> JoinGroup(Guid groupId)
    {
        User? user = await _userManager.GetUserAsync(new ClaimsPrincipal(User.Identity));
        if (user != null)
        {
            ChatGroup? requestedGroup = await _dataContext.ChatGroups.Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.Id == groupId);
            if (requestedGroup != null)
            {
                User? foundUser = requestedGroup.Users.FirstOrDefault(x => x.Id == user.Id);
                if (foundUser == null)
                {
                    _dataContext.ChatGroups.Update(requestedGroup);
                    requestedGroup.Users.Add(user);
                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    return RedirectToAction("FindGroup", "Home");
                }
            }
        }

        return RedirectToAction("Index", "Home", new {GroupId = groupId});
    }
}