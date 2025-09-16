using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.ViewModels.Crop;
using UHSB_Bagalkot.Service.ViewModels.Sections;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface IHorticultureHandbookRepository
    {
        Task<IEnumerable<UhsbCategory>> GetHorticultureHandbookAsync();  
        Task<IEnumerable<UhsbCrop>> GetHorticultureHandbookItemsAsync(int categoryId);
        #region Section Details
        Task<List<DropdownItemCropVM>> GetCropsForDD();
        Task<IEnumerable<UhsbSectionVM>> GetAllSectionsAsync();
        Task<UhsbSectionVM> GetSectionByIdAsync(int id);
        Task<bool> DeleteSectionAsync(int id);

        // WRITE
        Task<UhsbSectionVM> AddSectionAsync(UhsbSectionCreateUpdateVM model);
        Task<UhsbSectionVM?> UpdateSectionAsync(int id, UhsbSectionCreateUpdateVM model);
         
        #endregion
    }
}
