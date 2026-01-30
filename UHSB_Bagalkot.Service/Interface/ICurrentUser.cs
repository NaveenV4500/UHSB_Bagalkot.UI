using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface ICurrentUser
    {
        int UserId { get; }
        string UserName { get; }
        string Role { get; } 
    }
}
