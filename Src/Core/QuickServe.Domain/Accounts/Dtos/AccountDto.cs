using System;

namespace QuickServe.Domain.Accounts.Dtos
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Roles { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Avatar { get; set; } = null!;
        public DateTime Created { get; set; }
    }
}
