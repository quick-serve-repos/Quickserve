using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Interfaces
{
    public interface ISessionService
    {
        Task UpdateSessionStatus();
    }
}
