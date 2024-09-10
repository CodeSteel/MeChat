using System.ComponentModel.DataAnnotations;

namespace MeChat.Models.Requests;

public class CreateChatRequest
{
    [Required]
    public Guid GroupId { get; set; }

    [Required]
    public string Body { get; set; }

    public bool IsValid => !string.IsNullOrEmpty(Body.Trim()) && GroupId != Guid.Empty;
}