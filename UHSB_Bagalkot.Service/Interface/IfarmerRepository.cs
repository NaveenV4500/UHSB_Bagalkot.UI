using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface IfarmerRepository
    {
        Task<IEnumerable<FarmerProfile>> GetFarmerProfilesAsync();
        Task<FarmerProfile> GetFarmerProfileByIdAsync(int id);
        Task<FarmerProfile> AddFarmerProfileAsync(FarmerProfile farmerProfile);
        Task<bool> FarmerMobileExistsAsync(string mobile);
    }
}
