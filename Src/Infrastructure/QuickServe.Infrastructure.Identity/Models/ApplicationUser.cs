using Microsoft.AspNetCore.Identity;
using System;

namespace QuickServe.Infrastructure.Identity.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            Created = DateTime.UtcNow;

        }
        public string Name { get; set; }

        public DateTime Created { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
      //  public virtual ApplicationRole ApplicationRole { get; set; }
    }
}
