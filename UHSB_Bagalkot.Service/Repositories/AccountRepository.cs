using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Data.DataAccess;
using UHSB_Bagalkot.Service.Interface;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class AccountRepository :CommonConnection, IAccountRepository
{ 
        public AccountRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<UserMaster> GetUserByPhoneAsync(string phoneNumber)
        {
            return await _context.UserMasters
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<UserMaster> CreateUserAsync(UserMaster user)
        {
            _context.UserMasters.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
