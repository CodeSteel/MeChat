using System.ComponentModel.DataAnnotations;

namespace MeChat.Models.Requests;

public class CreateGroupRequest
{
    [Required]
    [MinLength(4)] 
    public string Name { get; set; }

    public bool IsValid => !string.IsNullOrEmpty(Name.Trim());
}