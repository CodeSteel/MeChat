namespace MeChat.Models;

public class DashboardResults
{
    public Guid Selection { get; set; }
    public IList<ChatGroup> GroupsWithUser { get; set; }
    public ChatGroup? SelectedGroup { get; set; }
}