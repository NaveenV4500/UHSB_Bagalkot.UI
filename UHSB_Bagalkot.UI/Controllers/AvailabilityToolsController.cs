using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.Repositories;
using UHSB_Bagalkot.Service.ViewModels;
using UHSB_Bagalkot.Service.ViewModels.AvailabilityTools;
using UHSB_Bagalkot.Service.ViewModels.Crop;
using UHSB_Bagalkot.Service.ViewModels.Product;

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
        public async Task<IActionResult> getgridcontentavailabilitytools(int currentPage = 1, int pageSize = 10,GridEnum.AvailabilityToolsFilterBy orderBy = GridEnum.AvailabilityToolsFilterBy.CreatedDate, bool isDescending = false,string filterDetails = null,string externalFilter = null, int centerid = 0, int districtid = 0, int pagetype = 0)
        {
            CommonEnum.WriteLog($"Availability Tools - API hit | DistrictId={districtid}, CenterId={centerid}, Page={currentPage}");
            var result = await _repository.getgridcontentavailabilitytools(currentPage, pageSize, orderBy, isDescending,filterDetails, externalFilter, centerid, districtid, pagetype);
            CommonEnum.WriteLog($"Availability Tools - Returning {result.TotalCount} rows");
            return Ok(new ApiResponse<object> {  Success = true, Data = result  });
        }

        [HttpGet("getgridcontentproducts")]
        public async Task<IActionResult> getgridcontentproducts(int currentPage = 1, int pageSize = 10, GridEnum.AvailabilityToolsFilterBy orderBy = GridEnum.AvailabilityToolsFilterBy.CreatedDate, bool isDescending = false, string filterDetails = null, string externalFilter = null, int centerid = 0, int districtid = 0, int pagetype = 0)
        {
            CommonEnum.WriteLog($"getgridcontentproducts   - API hit | DistrictId={districtid}, CenterId={centerid}, Page={currentPage}");
            var result = await _repository.getgridcontentproducts(currentPage, pageSize, orderBy, isDescending, filterDetails, externalFilter, centerid, districtid, pagetype);
            CommonEnum.WriteLog($"getgridcontentproducts  - Returning {result.TotalCount} rows");
            return Ok(new ApiResponse<object> { Success = true, Data = result });
        }

        [HttpGet("GetProductVarieties")]
        public async Task<IActionResult> GetProductVarieties(int productId = 0)
        {
             var result = await _repository.GetProductVarieties(productId);
             return Ok(new ApiResponse<object> { Success = true, Data = result });
        }
         


        [HttpGet("plantingcentersDD")]
        public async Task<IActionResult> plantingcentersDD()
        {
            var categories = await _repository.plantingcentersDD();
            return Ok(categories);
        }

        [HttpGet("recordheadtypeDD")]
        public async Task<IActionResult> recordheadtypeDD()
        {
            var categories = await _repository.RecordHeadTypeDD();
            return Ok(categories);
        }

         

        [HttpPost("SaveOrEditProdectDetails")]
        public async Task<IActionResult> SaveOrEditProdectDetails([FromBody] ProductsVM model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _repository.SaveOrEditProdectDetails(model);
            if (result == null) return NotFound("Update failed. Items not found.");

            return Ok(result);
        }

        [HttpGet("GetbyIdProdect")]
        public async Task<IActionResult> GetbyIdProdect(int identifier)
        {
 
            var result = await _repository.GetbyIdProdect(identifier);
            if (result == null) return NotFound("not found.");

            return Ok(result);
        }
     }
}
