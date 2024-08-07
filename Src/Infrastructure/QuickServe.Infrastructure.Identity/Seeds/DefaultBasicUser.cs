using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using QuickServe.Infrastructure.Identity.Models;
using System.Linq;
using System.Threading.Tasks;


namespace QuickServe.Infrastructure.Identity.Seeds
{
    public static class DefaultBasicUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
        //    // Seed Default Users
        //    var users = new List<ApplicationUser>
        //{
        //    new ApplicationUser
        //    {
        //        UserName = "Admin",
        //        Email = "sysadmin@quickserve.com",
        //        Name = "QuangVan",
        //        PhoneNumber = "0935182029",
        //        EmailConfirmed = true,
        //        PhoneNumberConfirmed = true
        //    },
        //    new ApplicationUser
        //    {
        //        UserName = "CustomerUser",
        //        Email = "customer@quickserve.com",
        //        Name = "Customer Name",
        //        PhoneNumber = "0935111111",
        //        EmailConfirmed = true,
        //        PhoneNumberConfirmed = true
        //    },
        //    new ApplicationUser
        //    {
        //        UserName = "StaffUser",
        //        Email = "staff@quickserve.com",
        //        Name = "Employee Name",
        //        PhoneNumber = "0935222222",
        //        EmailConfirmed = true,
        //        PhoneNumberConfirmed = true
        //    },
        //    new ApplicationUser
        //    {
        //        UserName = "StoreManagerUser",
        //        Email = "storemanager@quickserve.com",
        //        Name = "Store Manager Name",
        //        PhoneNumber = "0935333333",
        //        EmailConfirmed = true,
        //        PhoneNumberConfirmed = true
        //    },
        //    new ApplicationUser
        //    {
        //        UserName = "BrandManagerUser",
        //        Email = "brandmanager@quickserve.com",
        //        Name = "Brand Manager Name",
        //        PhoneNumber = "0935444444",
        //        EmailConfirmed = true,
        //        PhoneNumberConfirmed = true
        //    }
        //};

        //    var roles = new List<string> { "Admin", "Customer", "Employee", "Store_Manager", "Brand_Manager" };
        //    var userPasswords = new Dictionary<string, string>
        //{
        //    { "Admin", "Admin@123" },
        //    { "CustomerUser", "Customer@123" },
        //    { "StaffUser", "Employee@123" },
        //    { "StoreManagerUser", "StoreManager@123" },
        //    { "BrandManagerUser", "BrandManager@123" }
        //};

        //    foreach (var user in users)
        //    {
        //        if (!userManager.Users.Any(u => u.Email == user.Email))
        //        {
        //            var result = await userManager.CreateAsync(user, userPasswords[user.UserName]);
        //            if (result.Succeeded)
        //            {
        //                await userManager.AddToRoleAsync(user, roles[users.IndexOf(user)]);
        //            }
        //        }
        //    }
        }
    }



    //public static class DefaultBasicUser
    //{
    //    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    //    {
    //        // Seed Default Users
    //        var users = new List<ApplicationUser>
    //        {
    //            new ApplicationUser
    //            {
    //                UserName = "Admin",
    //                Email = "sysadmin@quickserve.com",
    //                Name = "QuangVan",
    //                PhoneNumber = "0935182029",
    //                EmailConfirmed = true,
    //                PhoneNumberConfirmed = true
    //            },
    //            new ApplicationUser
    //            {
    //                UserName = "CustomerUser",
    //                Email = "customer@quickserve.com",
    //                Name = "Customer Name",
    //                PhoneNumber = "0935111111",
    //                EmailConfirmed = true,
    //                PhoneNumberConfirmed = true
    //            },
    //            new ApplicationUser
    //            {
    //                UserName = "StaffUser",
    //                Email = "staff@quickserve.com",
    //                Name = "Staff Name",
    //                PhoneNumber = "0935222222",
    //                EmailConfirmed = true,
    //                PhoneNumberConfirmed = true
    //            },
    //            new ApplicationUser
    //            {
    //                UserName = "StoreManagerUser",
    //                Email = "storemanager@quickserve.com",
    //                Name = "Store Manager Name",
    //                PhoneNumber = "0935333333",
    //                EmailConfirmed = true,
    //                PhoneNumberConfirmed = true
    //            },
    //            new ApplicationUser
    //            {
    //                UserName = "BrandManagerUser",
    //                Email = "brandmanager@quickserve.com",
    //                Name = "Brand Manager Name",
    //                PhoneNumber = "0935444444",
    //                EmailConfirmed = true,
    //                PhoneNumberConfirmed = true
    //            }
    //        };

    //        var roles = new List<string> { "Admin", "Customer", "Staff", "Store_Manager", "Brand_Manager" };
    //        var userPasswords = new Dictionary<string, string>
    //        {
    //            { "Admin", "Admin@123" },
    //            { "CustomerUser", "Customer@123" },
    //            { "StaffUser", "Staff@123" },
    //            { "StoreManagerUser", "StoreManager@123" },
    //            { "BrandManagerUser", "BrandManager@123" }
    //        };

    //        foreach (var user in users)
    //        {
    //            if (!userManager.Users.Any(u => u.Email == user.Email))
    //            {
    //                var result = await userManager.CreateAsync(user, userPasswords[user.UserName]);
    //                if (result.Succeeded)
    //                {
    //                    var roleName = roles[users.IndexOf(user)];

    //                    // Tạo và liên kết vai trò với người dùng
    //                    var role = new ApplicationRole(roleName)
    //                    {
    //                        ApplicationUserId = user.Id,
    //                        ApplicationUser = user
    //                    };
    //                    await roleManager.CreateAsync(role);

    //                    // Gán vai trò cho người dùng
    //                    user.ApplicationRole = role;
    //                    await userManager.UpdateAsync(user);
    //                }
    //            }
    //        }
    //    }
    //}
}