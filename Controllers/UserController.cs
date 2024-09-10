using MeChat.Contexts;
using MeChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeChat.Controllers;

[Route("api")]
public class UserController : Controller
{
    private readonly ApplicationDataContext _dataContext;

    public UserController(ApplicationDataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    [HttpPost("user/friend/send-request")]
    [Authorize]
    public async Task<IActionResult> SendFriendRequest(Guid? userId, string? redirectUrl)
    {
        if (userId == null)
        {
            return BadRequest("userId is null.");
        }
        
        User? user = await _dataContext.Users
            .Include(x => x.UserRelationships)
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

        if (user == null)
        {
            return BadRequest("Failed to get request user.");
        }
        
        User? targetUser = await _dataContext.Users
            .Include(x => x.UserRelationships)
            .FirstOrDefaultAsync(x => x.Id == userId);
        
        if (targetUser == null)
        { 
            return BadRequest("Failed to get target user.");
        }

        UserRelationship? userRelationship = await _dataContext.UserRelationships
            .FirstOrDefaultAsync(x => x.OwnerId == user.Id && x.TargetUser.Id == targetUser.Id);
        
        if (userRelationship != null)
        {
            return BadRequest("UserRelationship already exists.");
        }

        await _dataContext.UserRelationships.AddAsync(new UserRelationship()
        {
            Owner = user,
            TargetUser = targetUser,
            Type = UserRelationshipType.FriendRequestSent,
        });
        
        await _dataContext.UserRelationships.AddAsync(new UserRelationship()
        {
            Owner = targetUser,
            TargetUser = user,
            Type = UserRelationshipType.FriendRequestReceived,
        });
        
        await _dataContext.SaveChangesAsync();
        
        return redirectUrl != null ? Redirect(redirectUrl) : Ok();
    }
    
    [HttpPost("user/friend/accept-request")]
    [Authorize]
    public async Task<IActionResult> AcceptFriendRequest(Guid? userId, string? redirectUrl)
    {
        if (userId == null)
        {
            return BadRequest("userId is null.");
        }
        
        User? user = await _dataContext.Users
            .Include(x => x.UserRelationships)
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

        if (user == null)
        {
            return BadRequest("Failed to get request user.");
        }
        
        User? targetUser = await _dataContext.Users
            .Include(x => x.UserRelationships)
            .FirstOrDefaultAsync(x => x.Id == userId.Value);
        
        if (targetUser == null)
        {
            return BadRequest("Failed to get target user.");
        }

        UserRelationship? userRelationship = await _dataContext.UserRelationships
            .FirstOrDefaultAsync(x => x.OwnerId == user.Id && x.TargetUser.Id == targetUser.Id);

        if (userRelationship is not { Type: UserRelationshipType.FriendRequestReceived })
        {
            return BadRequest("Request user doesn't have a pending friend request from target user.");
        }
        
        UserRelationship? targetUserRelationship = await _dataContext.UserRelationships
            .FirstOrDefaultAsync(x => x.OwnerId == targetUser.Id && x.TargetUser.Id == user.Id);

        if (targetUserRelationship is not { Type: UserRelationshipType.FriendRequestSent })
        {
            return BadRequest("Target user doesn't have a sent friend request to user.");
        }
        
        _dataContext.Update(userRelationship);
        _dataContext.Update(targetUserRelationship);

        userRelationship.Type = UserRelationshipType.Friend;
        targetUserRelationship.Type = UserRelationshipType.Friend;
        
        await _dataContext.SaveChangesAsync();
        
        return redirectUrl != null ? Redirect(redirectUrl) : Ok();
    }
    
    [HttpPost("user/friend/deny-request")]
    [Authorize]
    public async Task<IActionResult> DenyFriendRequest(Guid? userId, string? redirectUrl)
    {
        if (userId == null)
        {
            return BadRequest("userId is null.");
        }
        
        User? user = await _dataContext.Users
            .Include(x => x.UserRelationships)
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

        if (user == null)
        {
            return BadRequest("Failed to get request user.");
        }
        
        User? targetUser = await _dataContext.Users
            .Include(x => x.UserRelationships)
            .FirstOrDefaultAsync(x => x.Id == userId);
        
        if (targetUser == null)
        {
            return BadRequest("Failed to get target user.");
        }

        UserRelationship? userRelationship = user.UserRelationships
            .FirstOrDefault(x => x.TargetUser.Id == targetUser.Id);

        if (userRelationship is not { Type: UserRelationshipType.FriendRequestReceived })
        {
            return BadRequest("Request user doesn't have a pending friend request from target user.");
        }
        
        UserRelationship? targetUserRelationship = targetUser.UserRelationships
            .FirstOrDefault(x => x.TargetUser.Id == user.Id);
        
        if (targetUserRelationship is not { Type: UserRelationshipType.FriendRequestSent })
        {
            return BadRequest("Target user doesn't have a sent friend request to user.");
        }
        
        _dataContext.Remove(userRelationship);
        _dataContext.Remove(targetUserRelationship);
        
        await _dataContext.SaveChangesAsync();
        
        return redirectUrl != null ? Redirect(redirectUrl) : Ok();
    }
    
    [HttpPost("user/friend/remove")]
    [Authorize]
    public async Task<IActionResult> RemoveFriend(Guid? userId, string? redirectUrl)
    {
        if (userId == null)
        {
            return BadRequest("userId is null.");
        }
        
        User? user = await _dataContext.Users
            .Include(x => x.UserRelationships)
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

        if (user == null)
        {
            return BadRequest("Failed to get request user.");
        }
        
        User? targetUser = await _dataContext.Users
            .Include(x => x.UserRelationships)
            .FirstOrDefaultAsync(x => x.Id == userId);
        
        if (targetUser == null)
        {
            return BadRequest("Failed to get target user.");
        }

        UserRelationship? userRelationship = user.UserRelationships
            .FirstOrDefault(x => x.TargetUser.Id == targetUser.Id);

        if (userRelationship == null)
        {
            return BadRequest("Request user has no relationship with target user.");
        }
        
        UserRelationship? targetUserRelationship = targetUser.UserRelationships
            .FirstOrDefault(x => x.TargetUser.Id == user.Id);

        if (targetUserRelationship == null)
        {
            return BadRequest("Target user has no relationship with user.");
        }
        
        _dataContext.Remove(userRelationship);
        _dataContext.Remove(targetUserRelationship);
        
        await _dataContext.SaveChangesAsync();
        
        return redirectUrl != null ? Redirect(redirectUrl) : Ok();
    }
}