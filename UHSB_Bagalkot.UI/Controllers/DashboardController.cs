using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels;
using UHSB_Bagalkot.Service.ViewModels.AdminDashboard; 

namespace UHSB_Bagalkot.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _dashboardRepository.GetSummaryAsync();
            return Ok(summary);
        }

        [HttpGet("farmers-by-village")]
        public async Task<IActionResult> GetFarmersByVillage()
        {
            var result = await _dashboardRepository.GetFarmersByVillageAsync();
            return Ok(result);
        }

        [HttpGet("weekly-weather/{districtId}")]
        public async Task<IActionResult> GetWeeklyWeather(int districtId)
        {
            var result = await _dashboardRepository.GetWeeklyWeatherAsync(districtId);
            return Ok(result);
        }

        #region Crop Manage
        [HttpGet("Category")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _dashboardRepository.CategoryDD();
            return Ok(new ApiResponse<object> { Success = true, Data = result });
        }

        [HttpGet("Crops/{catId}")]
        public async Task<IActionResult> GetCrops(int catId)
        {
            if (catId <= 0) return BadRequest(new ApiResponse<object> { Success = false, Message = "Category Id is required" });

            var result = await _dashboardRepository.CropsDD(catId);
            return Ok(new ApiResponse<object> { Success = true, Data = result });
        }

        [HttpGet("Section/{cropId}")]
        public async Task<IActionResult> GetSections(int cropId)
        {
            if (cropId <= 0) return BadRequest(new ApiResponse<object> { Success = false, Message = "Crop Id is required" });

            var result = await _dashboardRepository.SectionDD(cropId);
            return Ok(new ApiResponse<object> { Success = true, Data = result });
        }

        [HttpGet("SubSection/{sectId}")]
        public async Task<IActionResult> GetSubSections(int sectId)
        {
            if (sectId <= 0) return BadRequest(new ApiResponse<object> { Success = false, Message = "Section Id is required" });

            var result = await _dashboardRepository.SubSectionDD(sectId);
            return Ok(new ApiResponse<object> { Success = true, Data = result });
        }

        [HttpGet("Items/{sectionId}/{cropId}")]
        public async Task<IActionResult> GetItems(int sectionId, int cropId)
        {
            if (sectionId <= 0 || cropId <= 0)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Both SectionId and CropId are required"
                });

            var result = await _dashboardRepository.ItemDeailsDD(sectionId, cropId);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = result
            });
        }


        [HttpGet("GetGridItemsV2")]
        public async Task<IActionResult> GetGridItemsV2(int currentPage = 1,  int pageSize = 10,  GridEnum.FTPDocumentsLogs orderBy = GridEnum.FTPDocumentsLogs.BranchName,  bool isDescending = false,  string filterDetails = null, string externalFilter = null, int subSectId = 0,int cropid=0)
        {
            if (subSectId <= 0)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "SubSection Id is required"
                });
             
            var result = await _dashboardRepository.GetGridItemsV2(
                currentPage, pageSize, orderBy, isDescending, filterDetails, 
                externalFilter, subSectId, cropid
            );

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("GetGridUsermasterV2")]
        public async Task<IActionResult> GetGridUsermasterV2(int currentPage = 1, int pageSize = 10, GridEnum.FTPDocumentsLogs orderBy = GridEnum.FTPDocumentsLogs.BranchName, bool isDescending = false, string filterDetails = null, string externalFilter = null)
        {

            var result = await _dashboardRepository.GetGridUsermasterV2(
                currentPage, pageSize, orderBy, isDescending, filterDetails, externalFilter
            );

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = result
            });
        }



        //save
        [HttpPost("SaveCropContent")] 
        public async Task<IActionResult> SaveCropContent([FromBody] List<UhsbItemImageVM> model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _dashboardRepository.SaveCropContentAsync(model);

            if (result) return Ok(new { success = true, message = "Saved successfully." });

            return StatusCode(500, new { success = false, message = "Error saving data." });
        }
        #endregion
    }
}
