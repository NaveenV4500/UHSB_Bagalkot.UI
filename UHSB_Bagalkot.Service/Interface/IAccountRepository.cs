using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface IAccountRepository
    {
        Task<UserMaster> GetUserByPhoneAsync(string phoneNumber);
        Task<UserMaster> CreateUserAsync(UserMaster user);

    }
}
