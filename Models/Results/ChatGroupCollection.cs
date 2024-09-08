namespace MeChat.Models;

public class ChatGroupCollection
{
    public string Search { get; set; }
    public ICollection<ChatGroup> ChatGroups { get; set; }
}