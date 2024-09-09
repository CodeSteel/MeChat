using System.ComponentModel.DataAnnotations;

namespace MeChat.Models;

public enum ChatGroupType
{
    DirectMessage,
    PublicGroup,
}

public class ChatGroup
{
    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public ChatGroupType Type { get; set; }
    
    public ICollection<Chat> Chats { get; } = new List<Chat>();

    public ICollection<User> Users { get; } = new List<User>();

    public User? Owner { get; set; } = null!;
}