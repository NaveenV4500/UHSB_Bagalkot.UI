using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels.Crop;
using UHSB_Bagalkot.Service.ViewModels.Sections;

namespace UHSB_Bagalkot.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class HorticultureHandbookController : ControllerBase
    {
        private readonly IHorticultureHandbookRepository _horticultureHandbookRepository;
        public HorticultureHandbookController (IHorticultureHandbookRepository horticultureHandbookRepository)
        {
            _horticultureHandbookRepository = horticultureHandbookRepository;
        }

        [HttpGet("getgridContentcategories")]
        public async Task<IActionResult> GetgridContentCategories()
        {
            var handbook = await _horticultureHandbookRepository.GetGridContentCategories();
            return Ok(handbook);
        }

        [HttpGet("getgridContentcrop")]
        public async Task<IActionResult> GetgridContentCrop()
        {
            var handbook = await _horticultureHandbookRepository.GetgridContentCrop();
            return Ok(handbook);
        }

        [HttpGet("GetgridContentSection")]
        public async Task<IActionResult> GetgridContentSection()
        {
            var handbook = await _horticultureHandbookRepository.GetgridContentSection();
            return Ok(handbook);
        }

        [HttpGet("GetgridContentItemDetails")]
        public async Task<IActionResult> GetgridContentItemDetails(int categoryId=0,int cropId=0,int sectionId=0)
        {
            var handbook = await _horticultureHandbookRepository.GetgridContentItemDetails(categoryId, cropId, sectionId);
            return Ok(handbook);
        }
        [HttpGet("items/{categoryId}")]
        public async Task<IActionResult> GetHorticultureHandbookItemsAsync(int categoryId)
        {
            var items = await _horticultureHandbookRepository.GetHorticultureHandbookItemsAsync(categoryId);
            return Ok(items);
        }

        [HttpGet("crops-dropdown")]
        public async Task<IActionResult> GetCropsForDropdown()
        {
            var crops = await _horticultureHandbookRepository.GetCropsForDD();
            return Ok(crops);
        }

        [HttpGet("GetgridContentSections")]
        public async Task<IActionResult> GetAllSectionsAsync()
        {
            var sections = await _horticultureHandbookRepository.GetAllSectionsAsync();
            return Ok(sections);
        }

        [HttpGet("sections/{id}")]
        public async Task<IActionResult> GetSectionByIdAsync(int id)
        {
            var section = await _horticultureHandbookRepository.GetSectionByIdAsync(id);
            if (section == null)
                return NotFound();

            return Ok(section);
        }

        [HttpDelete("sections/{id}")]
        public async Task<IActionResult> DeleteSectionAsync(int id)
        {
            var result = await _horticultureHandbookRepository.DeleteSectionAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("sections")]
        public async Task<IActionResult> AddSectionAsync([FromBody] UhsbSectionCreateUpdateVM model)
        {
            if (model == null)
                return BadRequest();

            var addedSection = await _horticultureHandbookRepository.AddSectionAsync(model);
            return Ok(addedSection);
        }

        [HttpPut("sections/{id}")]
        public async Task<IActionResult> UpdateSectionAsync(int id, [FromBody] UhsbSectionCreateUpdateVM model)
        {
            if (model == null)
                return BadRequest();

            var updatedSection = await _horticultureHandbookRepository.UpdateSectionAsync(id, model);

            if (updatedSection == null)
                return NotFound();

            return Ok(updatedSection);
        }
         
        [HttpPost("saveOrEditCrops")]
        public async Task<IActionResult> saveOrEditCrops([FromBody] CropDetailsVM model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _horticultureHandbookRepository.SaveOrEditCrops(model);
            if (result == null) return NotFound("Update failed. Crop not found.");

            return Ok(result);
        }
        [HttpPost("SaveOrEditItemDetails")]
        public async Task<IActionResult> SaveOrEditItemDetails([FromBody] RequestItemDetailsVM model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _horticultureHandbookRepository.SaveOrEditItemDetails(model);
            if (result == null) return NotFound("Update failed. Items not found.");

            return Ok(result);
        }


        [HttpPost("SaveOrEditSectionDetails")]
        public async Task<IActionResult> SaveOrEditSectionDetails([FromBody] RequestSectionDetailsVM model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _horticultureHandbookRepository.SaveOrEditSectionDetails(model);
            if (result == null) return NotFound("Update failed. Items not found.");

            return Ok(result);
        }

        [HttpPost("SaveOrEditCategoryDetails")]
        public async Task<IActionResult> SaveOrEditCategoryDetails([FromBody] RequestCategoryDetailsVM model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _horticultureHandbookRepository.SaveOrEditCategoryDetails(model);
            if (result == null) return NotFound("Update failed. Items not found.");

            return Ok(result);
        }
        
        [HttpPost("DeleteallpageItems")]
        public async Task<IActionResult> DeleteallpageItems([FromBody] DeleteItemVM delmodel)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _horticultureHandbookRepository.DeleteallpageItems(delmodel);

            return Ok(new
            {
                success = result.Success,
                message = result.Success
                       ? "Item(s) deleted successfully."
                       : (result.LinkedItems != null && result.LinkedItems.Count > 0
                           ? $"{string.Join(", ", result.LinkedItems)}"
                           : "Failed to delete item.")
            });
        }
    }
}
