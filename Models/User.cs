using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MeChat.Models;

public class User : IdentityUser<Guid>
{
    public string DisplayName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();

    public FileUpload ProfilePicture { get; set; } = null!;

    [NotMapped]
    public IFormFile ProfilePictureUpload { get; set; } = null!;
    
    public ICollection<User> Friends { get; } = new List<User>();
    
    public ICollection<User> FriendRequests { get; } = new List<User>();
    
    public ICollection<ChatGroup> ChatGroups { get; } = new List<ChatGroup>();
}