using Microsoft.AspNetCore.Identity;

namespace MeChat.Models;

public class User : IdentityUser<Guid>
{
    public string DisplayName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
    
    public ICollection<User> Friends { get; } = new List<User>();
    
    public ICollection<User> FriendRequests { get; } = new List<User>();
    
    public ICollection<ChatGroup> ChatGroups { get; } = new List<ChatGroup>();
}