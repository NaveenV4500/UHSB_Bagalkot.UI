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
         Task<IEnumerable<UhsbCrop>> GetHorticultureHandbookItemsAsync(int categoryId);

        Task<List<CategoryGridVM>> GetGridContentCategories();

        Task<List<CropGridVM>> GetgridContentCrop();

        Task<List<UhsbSectionVM>> GetgridContentSection();
        Task<List<ItemDetailsVM>> GetgridContentItemDetails(int categoryId = 0, int cropId = 0, int sectionId = 0);

        #region Section Details
        Task<List<DropdownItemCropVM>> GetCropsForDD();
        Task<IEnumerable<UhsbSectionVM>> GetAllSectionsAsync();
        Task<UhsbSectionVM> GetSectionByIdAsync(int id);
        Task<bool> DeleteSectionAsync(int id);

        // WRITE
        Task<UhsbSectionVM> AddSectionAsync(UhsbSectionCreateUpdateVM model);
        Task<UhsbSectionVM?> UpdateSectionAsync(int id, UhsbSectionCreateUpdateVM model);
        Task<bool> SaveOrEditCrops(CropDetailsVM cropVM);  
        Task<bool> SaveOrEditItemDetails(RequestItemDetailsVM cropVM);  
        Task<bool> SaveOrEditSectionDetails(RequestSectionDetailsVM SectionVM); 
        Task<bool> SaveOrEditCategoryDetails(RequestCategoryDetailsVM categoryVM);
        Task<(bool Success, List<string> LinkedItems)> DeleteallpageItems(DeleteItemVM delmodel);


        #endregion
    }
}
