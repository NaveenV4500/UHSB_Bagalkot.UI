using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Data.Models;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface IfarmerRepository
    {
        Task<IEnumerable<FarmersProfile>> GetFarmerProfilesAsync();
        Task<FarmersProfile> GetFarmerProfileByIdAsync(int id);
        Task<FarmersProfile> AddFarmerProfileAsync(FarmersProfile farmerProfile);
        Task<bool> FarmerMobileExistsAsync(string mobile);
    }
}
