using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using System.Threading.Tasks;

using QuickServe.Infrastructure.Identity.Models;

namespace QuickServe.Infrastructure.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager)
        {
            
            //// Danh sách các role cần được tạo
            //var roles = new List<string> { "Admin", "Customer", "Staff", "Store_Manager", "Brand_Manager" };

            //foreach (var role in roles)
            //{
            //    // Kiểm tra nếu role chưa tồn tại thì tạo mới
            //    if (!await roleManager.RoleExistsAsync(role))
            //    {
            //        await roleManager.CreateAsync(new ApplicationRole(role));
            //    }
            //}
        }
    }
}
