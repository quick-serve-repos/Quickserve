using Microsoft.AspNetCore.Identity;
using System;

namespace QuickServe.Infrastructure.Identity.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole(string name) : base(name)
        {
        }
        public virtual ApplicationUser User { get; set; }
    }
}