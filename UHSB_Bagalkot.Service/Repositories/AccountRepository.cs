using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class AccountRepository : CommonConnection, IAccountRepository
    {
        public AccountRepository(Uhsb2025uatContext context) : base(context)
        {
        }

        public async Task<UserMaster> GetUserByPhoneAsync(string phoneNumber, string userName = "", bool isFromAdmin = false)
        {
            return await _context.UserMasters
                .Where(u => u.PhoneNumber == phoneNumber && (!isFromAdmin || u.UserName == userName))
                .FirstOrDefaultAsync();
        }


        public async Task<bool> CreateOrUpdateUserAsync(UserMasterRequestmobile user)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    UserMaster existingUser = null;
                    FarmersProfile existingFarmerProfile = null;

                    // Check if user exists by Id or PhoneNumber
                    if (user.Id > 0)
                    {
                        existingUser = await _context.UserMasters
                            .FirstOrDefaultAsync(u => u.Id == user.Id);
                    }
                    else
                    {
                        existingUser = await _context.UserMasters
                            .FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
                    }

                    if (existingUser != null)
                    {
                        // Update existing user
                        existingUser.UserName = user.UserName;
                        existingUser.PhoneNumber = user.PhoneNumber;
                        existingUser.PasswordHash = user.PasswordHash;
                        existingUser.RoleType = user.RoleType;
                        existingUser.DistrictsId = (byte)user.DistrictsId;
                        existingUser.ModifiedDate = DateTime.UtcNow;
                        existingUser.ModifiedBy = 0;
                        existingUser.IsActive = user.IsActive;
                        existingUser.Village = user.Village;
                        existingUser.Address = user.Address;
                        existingUser.EmailId = user.EmailId;
                        existingUser.EmployeeId = user.EmployeeId;
                        _context.UserMasters.Update(existingUser);

                        //existingFarmerProfile = await _context.FarmersProfiles
                        //    .FirstOrDefaultAsync(f => f.Mobile == existingUser.PhoneNumber);

                        //if (existingFarmerProfile != null)
                        //{
                        //    existingFarmerProfile.Name = user.UserName;
                        //    existingFarmerProfile.LandSize = user.LandSize;
                        //    existingFarmerProfile.Village = user.Village;
 
                        //    _context.FarmersProfiles.Update(existingFarmerProfile);
                        //}
                        //else
                        //{
                        //    // If farmer profile doesn't exist, create new
                        //    var newFarmerProfile = new FarmersProfile
                        //    {
                        //        Name = user.UserName,
                        //        Mobile = user.PhoneNumber,
                        //        CreatedDate = DateTime.UtcNow,
                        //        LandSize = user.LandSize,
                        //        Village = user.Village
                        //    };
                        //    _context.FarmersProfiles.Add(newFarmerProfile);
                        //}
                    }
                    else
                    {
                        // Create new user
                        var newUser = new UserMaster
                        {
                            UserName = user.UserName,
                            PhoneNumber = user.PhoneNumber,
                            PasswordHash = user.PasswordHash,
                            RoleType = user.RoleType,
                            DistrictsId = (byte)user.DistrictsId, 
                            IsActive=user.IsActive,
                            CreatedAt = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow,
                            CreatedBy = 0,
                            ModifiedBy = 0,
                            Village = user.Village,
                            Address = user.Address,
                            EmailId=user.EmailId,
                            EmployeeId=user.EmployeeId
                        };
                        _context.UserMasters.Add(newUser);
                        await _context.SaveChangesAsync();

                        //var newFarmerProfile = new FarmersProfile
                        //{
                        //    Name = user.UserName,
                        //    Mobile = user.PhoneNumber,
                        //    CreatedDate = DateTime.UtcNow,
                        //    LandSize = user.LandSize,
                        //    Village = user.Village
                        //};
                        //_context.FarmersProfiles.Add(newFarmerProfile);
                    }

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


        public int GetUsersCount()
        {
            return _context.UserMasters.Count();
        }

        public Dictionary<int, string> GetAllUserRoleTypeAsDictionary()
        {
           

            return _context.UserRoles.Where(x=>x.Id != 1)
               .OrderBy(x => x.Id)
               .Select(x => new { x.Id, x.RoleName })
               .ToDictionary(x => x.Id, x => x.RoleName);

        }
        public Dictionary<int, string> GetAllDistrictsTypeAsDictionary()
        {
            return _context.UhsbDistricts
                .OrderBy(x => x.DistrictName)
                .Select(x => new { x.DistrictId, x.DistrictName })
                .ToDictionary(x => x.DistrictId, x => x.DistrictName);
        }

        //del usermaster
        public  bool DeleteUser(int userid, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                var delusers =   _context.UserMasters.Find(userid);
                if (delusers != null)
                {
                    _context.UserMasters.Remove(delusers);
                    _context.SaveChanges(); 
                    errorMessage = "User Delete Successfully";

                    return true;
                }
                errorMessage = "User not found";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public async Task SaveOtpAsync(int userId, int otp)
        {
            var entity = new UserOtp
            {
                UserId = userId,
                Otp = otp,
                ExpiryTime = DateTime.Now.AddMinutes(5), // OTP valid for 5 minutes
                IsUsed = false,
                CreatedOn = DateTime.Now
            };

            _context.UserOtps.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<UserOtp> GetLatestOtpAsync(int userId)
        {
            return await _context.UserOtps
                .Where(o => o.UserId == userId && !o.IsUsed)
                .OrderByDescending(o => o.CreatedOn)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateOtpAsync(UserOtp otpRecord)
        {
            _context.UserOtps.Update(otpRecord);
            await _context.SaveChangesAsync();
        }

    }
}
