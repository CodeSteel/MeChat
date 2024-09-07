using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MeChat.Services;

[Authorize]
public class ChatHub : Hub
{
    public async Task Send(Guid userId, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", userId, message);
    }
}
