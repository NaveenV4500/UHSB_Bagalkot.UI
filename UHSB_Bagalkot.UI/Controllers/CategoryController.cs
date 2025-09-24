using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICetegoryRepository _repository;

        public CategoryController(ICetegoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("getallCategory")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _repository.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("categoryby{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost("categoryAddOrEdit")]
        public async Task<IActionResult> AddOrEdit(CategoryVM category)
        {
            if (category == null)
                return BadRequest("Category data is required.");

            if (category.CategoryId == 0)
            {
                // Create new category
                var created = await _repository.AddAsync(category);
                return CreatedAtAction(nameof(GetById), new { id = created.CategoryId }, created);
            }
            else
            {
                // Update existing category
                var updated = await _repository.UpdateAsync(category);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
