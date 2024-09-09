namespace MeChat.Models.Requests;

public class CreateGroupRequest
{
    public string Name { get; set; }

    public bool IsValid => !string.IsNullOrEmpty(Name.Trim());
}