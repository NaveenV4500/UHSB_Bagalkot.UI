using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Service.ViewModels.AdminDashboard;
using UHSB_Bagalkot.WebService.ViewModels.ManageAdminDashboard;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryVM> GetSummaryAsync();
        Task<IEnumerable<object>> GetFarmersByVillageAsync();
        Task<IEnumerable<object>> GetWeeklyWeatherAsync(int districtId);

        #region Crop-Manage
        Task<IEnumerable<DropdownVM>> CategoryDD();
        Task<IEnumerable<DropdownVM>> CropsDD(int categoryId);
        Task<IEnumerable<DropdownVM>> SectionDD(int cropId);
        Task<IEnumerable<DropdownVM>> SubSectionDD(int sectionId);
        Task<IEnumerable<DropdownVM>> ItemDeailsDD(int subSectionId);
        Task<IEnumerable<UhsbItemImageVM>> GetgridItems(int subSectionId);

        //Save
        Task<bool> SaveCropContentAsync(List<UhsbItemImageVM> model);

        #endregion

    }
}
