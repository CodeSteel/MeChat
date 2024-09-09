using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeChat.Models;

public class Chat
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public User? PostedBy { get; set; } = null!;
    
    public string Body { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();

    [ForeignKey("GroupId")]
    public ChatGroup Group { get; set; } = null!;
    
    public Guid GroupId { get; set; }
}