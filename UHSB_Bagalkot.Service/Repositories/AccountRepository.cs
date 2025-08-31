using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Data.DataAccess;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class AccountRepository : CommonConnection, IAccountRepository
    {
        public AccountRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<UserMaster> GetUserByPhoneAsync(string phoneNumber)
        {
            return await _context.UserMasters
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<bool> CreateUserAsync(UserMasterVM user)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var users = new UserMaster
                    {
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        PasswordHash = user.PasswordHash,
                        RoleType = user.RoleType,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                    };
                    _context.UserMasters.Add(users);
                    await _context.SaveChangesAsync();

                    FarmerProfile farmerProfile = new FarmerProfile
                    {
                        Name = user.UserName,
                        Mobile = user.PhoneNumber,
                        CreatedDate = DateTime.UtcNow,
                        LandSize = user.LandSize,
                        Village = user.Village
                        
                    };

                    _context.FarmersProfiles.Add(farmerProfile);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }


    }
}
