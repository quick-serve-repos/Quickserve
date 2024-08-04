using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.Account.Responses
{
    public class ProfileResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Roles { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public int Status { get; set; }
    }
}
