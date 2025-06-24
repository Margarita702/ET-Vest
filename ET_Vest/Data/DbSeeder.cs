
using ET_Vest.Models;
using Microsoft.AspNetCore.Identity;

namespace ET_Vest.Data
{
    public class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            //Seed Roles
            var userManager = service.GetService<UserManager<ApplicationUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Owner.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Employee.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));

            // creating admin

            var user = new ApplicationUser
            {
                UserName = "owner@gmail.com",
                Email = "owner@gmail.com",
                FirstName = "Margarita",
                LastName="Staykova",
                Name="Margarita",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var userInDb = await userManager.FindByEmailAsync(user.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(user, "Owner@123");
                await userManager.AddToRoleAsync(user, Roles.Owner.ToString());
            }
        }
    }
}
