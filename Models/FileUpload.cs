using System.ComponentModel.DataAnnotations;

namespace MeChat.Models;

public class FileUpload
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Path { get; set; }
}