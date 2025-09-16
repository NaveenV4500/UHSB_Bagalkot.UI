using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Interface;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class farmerRepository : CommonConnection
    {
        public farmerRepository(Uhsb2025Context context) : base(context)
        {
        }
        public async Task<IEnumerable<FarmersProfile>> GetFarmerProfilesAsync()
        {
            return await _context.FarmersProfiles.ToListAsync();
        }

        public async Task<FarmersProfile> GetFarmerProfileByIdAsync(int id)
        {
            return await _context.FarmersProfiles.FindAsync(id);
        }

        public async Task<FarmersProfile> AddFarmerProfileAsync(FarmersProfile farmerProfile)
        {
            _context.FarmersProfiles.Add(farmerProfile);
            await _context.SaveChangesAsync();
            return farmerProfile;
        }

        public async Task<bool> FarmerMobileExistsAsync(string mobile)
        {
            return await _context.FarmersProfiles.AnyAsync(f => f.Mobile == mobile);
        }
    }
}
