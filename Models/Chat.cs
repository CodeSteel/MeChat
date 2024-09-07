using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeChat.Models;

public class Chat
{
    [Key]
    public Guid Id { get; set; }
    
    public User PostedBy { get; set; }
    
    public string Body { get; set; }
    
    public DateTime CreatedAt { get; set; }

    [ForeignKey("GroupId")]
    public ChatGroup Group { get; set; } = null!;
    
    public Guid GroupId { get; set; }
}