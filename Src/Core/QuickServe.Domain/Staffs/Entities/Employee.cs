using QuickServe.Domain.Accounts.Entities;
using QuickServe.Domain.Stores.Entities;
using System;

namespace QuickServe.Domain.Staffs.Entities
{
    public class Employee
    {
        public long StoreId { get; set; }
        public Guid EmployeeId { get; set; }

        public virtual Store Store { get; set; } = null!;
        public virtual Account Account { get; set; } = null!;
    }
}