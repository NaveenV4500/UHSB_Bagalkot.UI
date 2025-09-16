using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Service.Interface;
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

        [HttpGet("categories")]
        public async Task<IActionResult> GetHorticultureHandbook()
        {
            var handbook = await _horticultureHandbookRepository.GetHorticultureHandbookAsync();
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

        [HttpGet("sections")]
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
    }
}
