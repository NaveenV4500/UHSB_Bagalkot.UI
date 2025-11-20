using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.ViewModels.AvailabilityTools;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface IAvailabilityToolsRepository
    {
        Task<List<UhsbSeedPlantingCenterMasterVM>> GetCenterByDistrict(int districtid=0);
        Task<GenericGridModel<AvailabilityToolsDetailsVM>> getgridcontentavailabilitytools(int currentPage = 1, int pageSize = 10, GridEnum.AvailabilityToolsFilterBy orderBy = GridEnum.AvailabilityToolsFilterBy.CreatedDate, bool isDescending = false, string filterDetails = null, string externalFilter = null, int centerid = 0, int districtid = 0, int pagetype = 0);

    }
}
