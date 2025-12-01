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
        Task<bool> CreateOrUpdateUserAsync(UserMasterRequestmobile user);
        int GetUsersCount();
        public Dictionary<int, string> GetAllUserRoleTypeAsDictionary();
        public Dictionary<int, string> GetAllDistrictsTypeAsDictionary();

         //del usermaster
        bool DeleteUser(int userid, out string errorMessage);

        //OTP
        Task SaveOtpAsync(int userId, int otp);
        Task<UserOtp> GetLatestOtpAsync(int userId);
        Task UpdateOtpAsync(UserOtp otpRecord); 

    }
}
