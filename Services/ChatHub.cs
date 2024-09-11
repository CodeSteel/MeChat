using MeChat.Contexts;
using MeChat.Controllers;
using MeChat.Models;
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

    public async Task Send(Guid groupId, string message)
    {
        User? user = await _dataContext.Users
            .Include(x => x.ProfilePicture)
            .FirstOrDefaultAsync(x => x.Email == Context.User.Identity.Name);
        if (user != null)
        {
            await Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", user.DisplayName, user.Id, user.ProfilePicture.Path, DateTime.Now.ToUniversalTime().ToString("M/d/yyyy h:mm:ss tt"), message, groupId);
        }
    }

    public async Task AddToGroup(string groupId)
    {
        if (SignalRConnectionToGroupsMap.TryRemoveConnection(Context.ConnectionId,
                out List<string> pastGroups))
        {
            foreach (string groupStr in pastGroups)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupStr);
            }
        }

        if (SignalRConnectionToGroupsMap.TryAddGroup(Context.ConnectionId, groupId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }
    }
}
