using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.ViewModels;
using UHSB_Bagalkot.Service.ViewModels.AdminDashboard;  

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
        Task<IEnumerable<DropdownVM>> ItemDeailsDD(int sectionId,int cropId);
        Task<GenericGridModel<UhsbItemImageVM>> GetGridItemsV2(int currentPage = 1, int pageSize = 10, GridEnum.FTPDocumentsLogs orderBy = GridEnum.FTPDocumentsLogs.BranchName, bool isDescending = false, string filterDetails = null, string externalFilter = null, int subSectId = 0, int cropid = 0,int categoryid=0);
        Task<GenericGridModel<UserMasterVM>> GetGridUsermasterV2(int currentPage = 1, int pageSize = 10, GridEnum.UserMasterColumns orderBy = GridEnum.UserMasterColumns.CreatedDate, bool isDescending = false, string filterDetails = null, string externalFilter = null);
        //edit 
        Task<UhsbItemImageVM> GetByIdImageConentDetails(int imagecontentid);

        //Save
        Task<bool> SaveCropContentAsync(List<UhsbItemImageVM> model);

       
        #endregion

    }
}
