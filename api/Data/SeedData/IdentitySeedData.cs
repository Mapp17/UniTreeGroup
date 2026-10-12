using Microsoft.AspNetCore.Identity;

public static class IdentitySeedData
{
    public const string Admin = "Admin";
    public const string SysUsers = "User";

    public static async Task SeedIdentityAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        string[] roles = {Admin, SysUsers};

        foreach(var role in roles)
        {
            if(!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        //await CreateUser(userManager,"admin@unitree.com","Admin123!",Admin);
        //await CreateUser(userManager,"user1@unitree.com", "user123!", SysUsers);
        
    }
    private static async Task CreateUser(UserManager<ApplicationUser> userManager, string email, string password, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if(user != null) return;

        user = new ApplicationUser
        {
            Email = email  
        };

        var result = await userManager.CreateAsync(user, password);
        if(result.Succeeded)
        { 
            await userManager.AddToRoleAsync(user, role);
        }
    }
}

public class ApplicationUser : IdentityUser
{
    
}