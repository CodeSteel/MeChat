using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeChat.Models;

public enum UserRelationshipType
{
    FriendRequestSent,
    FriendRequestReceived,
    Friend,
}

public class UserRelationship
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public User TargetUser { get; set; } = null!;
    
    public UserRelationshipType Type { get; set; }

    [ForeignKey("OwnerId")]
    public User Owner { get; set; } = null!;
    
    public Guid OwnerId { get; set; }
}