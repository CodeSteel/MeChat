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
    public async Task<IActionResult> SendMessage(CreateChatRequest? request, string? redirectUrl)
    {
        if (request == null || !request.IsValid)
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
        
        AppStatistic stats = await _dataContext.AppStatistics.FirstAsync();
        _dataContext.Update(stats);
        
        stats.ChatsCreated++;

        await _dataContext.Chats.AddAsync(newChat);
        await _dataContext.SaveChangesAsync();

        return redirectUrl != null ? Redirect(redirectUrl) : Ok();
    }
    
    [HttpPost("group/create")]
    [Authorize]
    public async Task<IActionResult> CreateGroup(CreateGroupRequest? request, string? redirectUrl)
    {
        if (request == null || !request.IsValid)
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
            Name = request.Name,
            Type = ChatGroupType.PublicGroup,
            Owner = requestUser
        };
        newChatGroup.Users.Add(requestUser);
        
        Chat newGroupChat = new Chat()
        {
            Group = newChatGroup,
            Body = $"{requestUser.DisplayName} created this goup."
        };
            
        AppStatistic stats = await _dataContext.AppStatistics.FirstAsync();
        _dataContext.Update(stats);
        
        stats.GroupsCreated++;

        await _dataContext.ChatGroups.AddAsync(newChatGroup);
        await _dataContext.Chats.AddAsync(newGroupChat);
        await _dataContext.SaveChangesAsync();
        
        return redirectUrl != null ? Redirect(redirectUrl + newChatGroup.Id) : Ok();
    }
    
    [HttpPost("group/leave")]
    [Authorize]
    public async Task<IActionResult> LeaveGroup(Guid? groupId, string? redirectUrl)
    {
        if (groupId == null || groupId.Value == Guid.Empty)
        {
            return BadRequest("GroupId is invalid.");
        }
        
        User? user = await _userManager.GetUserAsync(new ClaimsPrincipal(User.Identity));
        if (user == null)
        {
            return BadRequest("User is invalid.");
        }

        ChatGroup? requestedGroup = await _dataContext.ChatGroups
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == groupId);

        if (requestedGroup == null)
        {
            return BadRequest("Request group is invalid.");
        }

        if (requestedGroup is { Type: ChatGroupType.DirectMessage })
        {
            return BadRequest("Can't leave direct message groups.");
        }

        User? foundUser = requestedGroup.Users.FirstOrDefault(x => x.Id == user.Id);
        if (foundUser == null)
        {
            return BadRequest("User not found.");
        }

        if (requestedGroup.Users.Count > 1)
        {
            _dataContext.ChatGroups.Update(requestedGroup);
            requestedGroup.Users.Remove(user);
            
            Chat leaveChat = new Chat()
            {
                Group = requestedGroup,
                Body = $"{user.DisplayName} left the group."
            };

            await _dataContext.Chats.AddAsync(leaveChat);
        }
        else
        {
            _dataContext.ChatGroups.Remove(requestedGroup);
        }
        
        await _dataContext.SaveChangesAsync();
        
        return redirectUrl != null ? Redirect(redirectUrl) : Ok();
    }
    
    [HttpPost("group/join")]
    [Authorize]
    public async Task<IActionResult> JoinGroup(Guid? groupId, string? redirectUrl)
    {
        if (groupId == null || groupId.Value == Guid.Empty)
        {
            return BadRequest("GroupId is invalid.");
        }
        
        User? user = await _userManager.GetUserAsync(new ClaimsPrincipal(User.Identity));
        if (user == null)
        {
            return BadRequest("User is invalid.");
        }
        
        ChatGroup? requestedGroup = await _dataContext.ChatGroups
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == groupId);
        
        if (requestedGroup != null)
        {
            User? foundUser = requestedGroup.Users.FirstOrDefault(x => x.Id == user.Id);
            if (foundUser == null)
            {
                _dataContext.ChatGroups.Update(requestedGroup);
                requestedGroup.Users.Add(user);

                Chat welcomeChat = new Chat()
                {
                    Group = requestedGroup,
                    Body = $"{user.DisplayName} joined the group."
                };

                await _dataContext.Chats.AddAsync(welcomeChat);
                await _dataContext.SaveChangesAsync();
            }
            else
            {
                return redirectUrl != null ? Redirect(redirectUrl) : Ok();
            }
        }

        return redirectUrl != null ? Redirect(redirectUrl) : Ok();
    }
}