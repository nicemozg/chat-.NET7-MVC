using Microsoft.AspNetCore.Identity;

namespace Chat.Models;

public class User : IdentityUser
{
    public string AvatarFileName { get; set; }
    public DateTime DateOfBirth { get; set; }

    public User()
    {
    }
}