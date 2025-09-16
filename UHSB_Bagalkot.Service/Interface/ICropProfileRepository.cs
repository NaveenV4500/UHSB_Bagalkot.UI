using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Dto;
using UHSB_Bagalkot.Service.ViewModels;
using UHSB_Bagalkot.Service.ViewModels.CropProfile;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface ICropProfileRepository
    {
        IEnumerable<UhsbCategory> GetCategoryItems();
        IEnumerable<UhsbCrop> GetCropDetailsAsync(int categoryId);
        IEnumerable<UhsbSection> GetSectionsByCropId(int cropId);
        IEnumerable<UhsbSubSection> GetSubSectionsBySectionId(int sectionId);
        Task<IEnumerable<UhsbItemDeail>> GetItemsBySubSectionIdAsync(int subSectionId);
        Task<List<ItemDto>> GetContentByItemIdAsync(int itemId);


        Task<IEnumerable<UhsbItemQnA>> GetByItemIdAsync(int itemId);
        Task<UhsbItemQnA> AddAsync(UhsbItemQnAVM qna);
        //public Task AddCropFullDetailsAsync(CropFullDetailsDto cropDetails);
        public Task<List<DropdownItemDto>> GetCategoryForDD();

        Task<List<FarmersProfile>> GetAllFarmers();

    }
}
