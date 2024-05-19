using Chat.Models;
using Microsoft.AspNetCore.Identity;

namespace Chat.Services;

public class AdminInitializer
{
    public static async Task SeedAdminUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        string adminEmail = "admin@admin.com";
        string adminUserName = "admin";
        string password = "Qwerty123@";
        DateTime dateOfBirth = new DateTime(2000, 1, 1);

        var roles = new[] { "admin", "user" };
        foreach (var role in roles)
        {
            if (await roleManager.FindByNameAsync(role) is null)
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        if (await userManager.FindByNameAsync(adminEmail) is null)
        {
            User admin = new User { Email = adminEmail, UserName = adminUserName, DateOfBirth  = dateOfBirth,  AvatarFileName = "d16c062c-6fa8-44f1-9524-8ce7b9a5bd2e_User.jpg"};
            IdentityResult result = await userManager.CreateAsync(admin, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "admin");
        }
    }
}