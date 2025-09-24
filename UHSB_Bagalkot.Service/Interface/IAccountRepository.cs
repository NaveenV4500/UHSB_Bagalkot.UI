using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface IAccountRepository
    {
        Task<UserMaster> GetUserByPhoneAsync(string phoneNumber, string userName = "", bool isFromAdmin = false);
        Task<bool> CreateUserAsync(UserMasterVM user);
        int GetUsersCount();
        
    }
}
