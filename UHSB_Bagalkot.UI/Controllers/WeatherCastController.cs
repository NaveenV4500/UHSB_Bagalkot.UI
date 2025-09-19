using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherCastController : ControllerBase
    {
        private readonly IWeatherCastRepository _repository;

        public WeatherCastController(IWeatherCastRepository service)
        {
            _repository = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _repository.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _repository.GetByIdAsync(id);
            if (record == null) return NotFound();
            return Ok(record);
        }


        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromBody] WeatherFileUploadVM dto)
        {
            if (dto.FileBytes == null || dto.FileBytes.Length == 0)
                return BadRequest("File is required.");

            var result =   _repository.SaveFileAsync(dto);
            return Ok(result);
        }

        

        [HttpGet("Weekly")]
        public async Task<IActionResult> GetWeeklyReports(int districtId)
        {
            if (districtId <= 0)
                return BadRequest("Invalid DistrictId.");

            var reports = await _repository.GetWeeklyReportsAsync(districtId);

            if (reports == null || !reports.Any())
                return NotFound("No reports found for the given district.");

           
            var today = DateTime.Now;
            var currentWeek = CommonHelper.GetWeekRange(today);
            var previousWeek = CommonHelper.GetPreviousWeekRange(today);

            var result = reports.Select(r => new
            {
                
                r.DistrictId,
                r.FilePath,
                r.Description,
                r.WeekStartDate,
                r.WeekEndDate,
                r.CreatedDate,
                WeekType = (r.WeekStartDate <= DateOnly.FromDateTime(currentWeek.endOfWeek) && r.WeekEndDate >= DateOnly.FromDateTime(currentWeek.startOfWeek))
                           ? "CurrentWeek"
                           : "PreviousWeek"
            });

            return Ok(result);
        }



    }
}
