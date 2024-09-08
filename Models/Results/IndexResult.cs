namespace MeChat.Models;

public class IndexResult
{
    public Guid Selection { get; set; }
    public IList<ChatGroup> GroupsWithUser { get; set; }
    public ChatGroup SelectedGroup { get; set; }
}