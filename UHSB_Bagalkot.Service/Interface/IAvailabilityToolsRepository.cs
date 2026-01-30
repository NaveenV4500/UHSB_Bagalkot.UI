using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.Dto;
using UHSB_Bagalkot.Service.ViewModels;
using UHSB_Bagalkot.Service.ViewModels.AvailabilityTools;
using UHSB_Bagalkot.Service.ViewModels.Product;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface IAvailabilityToolsRepository
    {
        Task<List<UhsbSeedPlantingCenterMasterVM>> GetCenterByDistrict(int districtid=0);
        Task<GenericGridModel<AvailabilityToolsDetailsVM>> getgridcontentavailabilitytools(int currentPage = 1, int pageSize = 10, GridEnum.AvailabilityToolsFilterBy orderBy = GridEnum.AvailabilityToolsFilterBy.CreatedDate, bool isDescending = false, string filterDetails = null, string externalFilter = null, int centerid = 0, int districtid = 0, int pagetype = 0);
        Task<GenericGridModel<Product_SP_VM>> getgridcontentproducts(int currentPage = 1, int pageSize = 10, GridEnum.AvailabilityToolsFilterBy orderBy = GridEnum.AvailabilityToolsFilterBy.CreatedDate, bool isDescending = false, string filterDetails = null, string externalFilter = null, int centerid = 0, int districtid = 0, int pagetype = 0);
        public Task<List<DropdownItemDto>> plantingcentersDD();
        public Task<List<DropdownItemDto>> RecordHeadTypeDD();
        Task<List<ProductVariety_SP_VM>> GetProductVarieties(int productid = 0);

        Task<saveresponse> SaveOrEditProdectDetails(ProductsVM obj);
        Task<ProductsVM?> GetbyIdProdect(int identifier);
    }
}
