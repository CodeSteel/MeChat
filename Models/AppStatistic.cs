using System.ComponentModel.DataAnnotations;

namespace MeChat.Models;

public class AppStatistic
{
    [Key]
    public int Id;
    
    public int UsersCreated { get; set; }
    
    public int GroupsCreated { get; set; }
    
    public int ChatsCreated { get; set; }
}