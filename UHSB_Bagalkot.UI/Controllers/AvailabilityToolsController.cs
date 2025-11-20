using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.Repositories;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityToolsController : ControllerBase
    {
        private readonly IAvailabilityToolsRepository _repository;

        public AvailabilityToolsController(IAvailabilityToolsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("getcenterbydistrict")]
        public IActionResult GetCenterByDistrict(int districtid = 0)
        {
            return _repository.GetCenterByDistrict(districtid).Result != null
                ? Ok(_repository.GetCenterByDistrict(districtid).Result)
                : NotFound();
        }

        [HttpGet("getgridcontentavailabilitytools")]
        public async Task<IActionResult> getgridcontentavailabilitytools(int currentPage = 1, int pageSize = 10, GridEnum.AvailabilityToolsFilterBy orderBy = GridEnum.AvailabilityToolsFilterBy.CreatedDate, bool isDescending = false, string filterDetails = null, string externalFilter = null, int centerid = 0, int districtid = 0, int pagetype = 0)
        {
            var result = await _repository.getgridcontentavailabilitytools(
                currentPage, pageSize, orderBy, isDescending, filterDetails,
                externalFilter, centerid, districtid, pagetype
            );

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = result
            });
        }

    }
}
