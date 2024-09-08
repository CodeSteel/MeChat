namespace MeChat.Models;

public class FindGroupResult
{
    public string Search { get; set; }
    public ICollection<ChatGroup> ChatGroups { get; set; }
}