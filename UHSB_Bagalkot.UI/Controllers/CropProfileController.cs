using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UHSB_Bagalkot.Service.Dto;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.Repositories;
using UHSB_Bagalkot.Service.ViewModels.CropProfile;

namespace UHSB_Bagalkot.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CropProfileController : ControllerBase
    {
        
        private readonly ICropProfileRepository _repository;
        public CropProfileController(ICropProfileRepository cropProfileRepository)
        {
            _repository = cropProfileRepository;
        }

        [HttpGet("GetCategoryDetails")]
        public IActionResult GetCategoryItems()
        {
            var categories = _repository.GetCategoryItems();
            return Ok(categories);
        }

        [HttpGet("GetCropDetails/{categoryId}")]
        public IActionResult GetCropDetails(int categoryId)
        {
            var crops = _repository.GetCropDetailsAsync(categoryId);
            return Ok(crops);
        }

        [HttpGet("GetSectionsByCrop/{cropId}")]
        public IActionResult GetSectionsByCropId(int cropId)
        {
            var sections = _repository.GetSectionsByCropId(cropId);
            return Ok(sections);
        }

        [HttpGet("GetSubSectionsBySection/{sectionId}")]
        public IActionResult GetSubSectionsBySectionId(int sectionId)
        {
            var subSections = _repository.GetSubSectionsBySectionId(sectionId);
            return Ok(subSections);
        }

        [HttpGet("GetByItemDetails/{subSectionId}")]
        public async Task<IActionResult> GetItemsBySubSectionIdAsync(int subSectionId)
        {
            var items = await _repository.GetItemsBySubSectionIdAsync(subSectionId);
            return Ok(items);
        }



        [HttpGet("Content/{itemId}")]
        public async Task<IActionResult> GetContentByItemIdAsync(int itemId)
        {
            var content = await _repository.GetContentByItemIdAsync(itemId);
            return Ok(content);
        }

        [HttpGet("GetByQuestionItem/{itemId}")]
        public async Task<IActionResult> GetByItemIdAsync(int itemId)
        {
            var qnas = await _repository.GetByItemIdAsync(itemId);
            return Ok(qnas);
        }

        [HttpPost("SaveQuestionItem")]
        public async Task<IActionResult> AddQnA([FromBody] UhsbItemQnAVM qnaVm)
        {
            if (qnaVm == null)
                return BadRequest();

            var addedQnA = await _repository.AddAsync(qnaVm);
            return Ok(addedQnA);
        }

        [HttpGet("categoriesDD")]
        public async Task<IActionResult> GetCategoryForDropdown()
        {
            var categories = await _repository.GetCategoryForDD();
            return Ok(categories);
        }


        [HttpGet("GetAllRegisterFarmers")]
        public async Task<IActionResult> GetAllFarmers()
        {
            var farmers = _repository.GetAllFarmers();
            return Ok(farmers);
        }


        

        //[HttpGet("full-details/{cropId}")]
        //public async Task<IActionResult> GetCropFullDetailsAsync(int cropId)
        //{
        //    var fullDetails = await _repository.GetCropFullDetailsAsync(cropId);
        //    return Ok(fullDetails);
        //}
    }
}
