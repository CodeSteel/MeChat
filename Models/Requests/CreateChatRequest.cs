namespace MeChat.Models.Requests;

public class CreateChatRequest
{
    public Guid GroupId { get; set; }

    public string Body { get; set; }
}