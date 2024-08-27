using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using QuickServe.Domain.Orders.Entities;
using QuickServe.Domain.Staffs.Entities;


namespace QuickServe.Domain.Accounts.Entities
{
    public class Account :  IdentityUser<Guid>
    {
        public Account()
        {
            Created = DateTime.Now;
        }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public int Status { get; set; }
        public virtual Employee Staff { get; set; } = null!;
    }
}