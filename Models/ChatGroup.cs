using System.ComponentModel.DataAnnotations;

namespace MeChat.Models;

public class ChatGroup
{
    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public bool Public { get; set; }

    public ICollection<Chat> Chats { get; } = new List<Chat>();

    public ICollection<User> Users { get; } = new List<User>();
}