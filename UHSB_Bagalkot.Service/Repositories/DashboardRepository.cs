using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels;
using UHSB_Bagalkot.Service.ViewModels.AdminDashboard;
using UHSB_Bagalkot.WebService.ViewModels.ManageAdminDashboard;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class DashboardRepository : CommonConnection, IDashboardRepository
    {

        public DashboardRepository(Uhsb2025Context context) : base(context)
        {

        }

        public async Task<DashboardSummaryVM> GetSummaryAsync()
        {
            return new DashboardSummaryVM
            {
                TotalUsers = await _context.UserMasters.CountAsync(),
                ActiveUsers = await _context.UserMasters.CountAsync(u => u.IsActive),
                Farmers = await _context.FarmersProfiles.CountAsync(),
                Categories = await _context.UhsbCategories.CountAsync(),
                Crops = await _context.UhsbCrops.CountAsync(),
                WeatherFiles = await _context.UhsbWeatherCastFileDetails.CountAsync()
            };
        }

        public async Task<IEnumerable<object>> GetFarmersByVillageAsync()
        {
            return await _context.FarmersProfiles
                .GroupBy(f => f.Village)
                .Select(g => new { Village = g.Key, Count = g.Count() })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetWeeklyWeatherAsync(int districtId)
        {
            return await _context.UhsbWeatherCastFileDetails
                .FromSqlRaw("EXEC sp_GetWeeklyWeatherRecords @DistrictId",
                    new SqlParameter("@DistrictId", districtId))
                .ToListAsync();
        }


        #region Crop Manage
        public async Task<IEnumerable<DropdownVM>> CategoryDD()
        {
            return await _context.UhsbCategories
                .Select(c => new DropdownVM { Id = c.CategoryId, Name = c.Name })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownVM>> CropsDD(int categoryId)
        {
            return await _context.UhsbCrops
                .Where(c => c.CategoryId == categoryId)
                .Select(c => new DropdownVM { Id = c.CropId, Name = c.Name })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownVM>> SectionDD(int cropId)
        {
            return await _context.UhsbSections
                .Where(s => s.CropId == cropId)
                .Select(s => new DropdownVM { Id = s.SectionId, Name = s.Name })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownVM>> SubSectionDD(int sectionId)
        {
            return await _context.UhsbSubSections
                .Where(ss => ss.SectionId == sectionId)
                .Select(ss => new DropdownVM { Id = ss.SubSectionId, Name = ss.Name })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownVM>> ItemDeailsDD(int subSectionId)
        {
            return await _context.UhsbItemDeails
                .Where(i => i.SubSectionId == subSectionId)
                .Select(i => new DropdownVM { Id = i.ItemId, Name = i.Name })
                .ToListAsync();
        }

        //save
        public async Task<bool> SaveCropContentAsync(List<UhsbItemImageVM> model)
        {
            if (model == null) return false;
               
            foreach (var item in model)
            { 
                var cropItem = new UhsbItemImage
                { 
                    ItemId = item.ItemId,
                    Description = item.Description,
                    ImageUrl = item.ImageUrl
                };

                _context.UhsbItemImages.Add(cropItem);
            }
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion
    }
}
