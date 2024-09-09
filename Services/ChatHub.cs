using System.Security.Claims;
using MeChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace MeChat.Services;

public class ChatHub : Hub
{
    private readonly UserManager<User> _userManager;
    
    public ChatHub(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task Send(string message)
    {
        User? user = await _userManager.GetUserAsync(new ClaimsPrincipal(Context.User.Identity));
        if (user != null)
        {
            await Clients.All.SendAsync("ReceiveMessage", user.DisplayName, user.Id, DateTime.Now.ToUniversalTime().ToString("M/d/yyyy h:mm:ss tt"), message);
        }
    }
}
