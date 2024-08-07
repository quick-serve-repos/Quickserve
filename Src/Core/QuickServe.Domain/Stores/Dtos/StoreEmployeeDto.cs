using QuickServe.Domain.Accounts.Dtos;
using System;
using System.Collections.Generic;

namespace QuickServe.Domain.Stores.Dtos
{
    public class StoreEmployeeDto
    {
        public List<EmployeeDto>? Employees { get; set; }
    }

    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; } 
        public string? Email { get; set; } 
        public string? Roles { get; set; } 
        public string? PhoneNumber { get; set; }
        public string? Name { get; set; } 
        public string?  Address { get; set; } 
        public string? Avatar { get; set; }
        public DateTime Created { get; set; }
    }
}
