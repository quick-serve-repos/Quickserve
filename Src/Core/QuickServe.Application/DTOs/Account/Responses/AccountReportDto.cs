using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.Account.Responses
{
    public class AccountReportDto
    {
        public int TotalAccounts { get; set; }
        public Dictionary<string, int> AccountsByRole { get; set; }
        public int TotalEmployeeCount { get; set; }
        public int EmployeeByStoreCount { get; set; }

        public AccountReportDto()
        {
            AccountsByRole = new Dictionary<string, int>();
        }
    }
}
