using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UHSB_Bagalkot.Service.Dto;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.Repositories;

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

        public async Task<CropFullDetailsDto> GetCropFullDetailsAsync(int cropId)
        {
            return await _repository.GetCropFullDetailsAsync(cropId);
        }
    }
}
