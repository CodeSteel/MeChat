using System.Security.Claims;
using MeChat.Contexts;
using MeChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MeChat.Services;

public class ChatHub : Hub
{
    private readonly ApplicationDataContext _dataContext;
    
    public ChatHub(ApplicationDataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task Send(string message)
    {
        User? user = await _dataContext.Users
            .Include(x => x.ProfilePicture)
            .FirstOrDefaultAsync(x => x.Email == Context.User.Identity.Name);
        if (user != null)
        {
            await Clients.All.SendAsync("ReceiveMessage", user.DisplayName, user.Id, user.ProfilePicture.Path, DateTime.Now.ToUniversalTime().ToString("M/d/yyyy h:mm:ss tt"), message);
        }
    }
}
