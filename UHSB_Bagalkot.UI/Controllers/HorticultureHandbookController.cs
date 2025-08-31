using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Service.Interface;

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
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CropCategory>>> GetHorticultureHandbook()
        {
            var handbook = await _horticultureHandbookRepository.GetHorticultureHandbookAsync();
            return Ok(handbook);
        }

        [HttpGet]
        [Route("GetHorticultureHandbookItemsById/{categoryId}")]
        public async Task<ActionResult<Crop>> GetHorticultureHandbookById(int categoryId)
        {
            var handbookItems = await _horticultureHandbookRepository.GetHorticultureHandbookItemsAsync(categoryId);

            return Ok(handbookItems);
        }
    }
}
