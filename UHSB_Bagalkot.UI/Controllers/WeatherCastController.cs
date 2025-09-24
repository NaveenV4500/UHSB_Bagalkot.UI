using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherCastController : ControllerBase
    {
        private readonly IWeatherCastRepository _repository;
        private readonly IWebHostEnvironment _env;
        
        public WeatherCastController(IWeatherCastRepository service, IWebHostEnvironment env)
        {
            _env = env; 
            _repository = service;
        }
        private string GetFilePath(string baseFolder, string fileName, string defaultExtension = ".pdf")
        { 
            string serverRoot = _env.ContentRootPath;  

            var folderPath = Path.Combine(serverRoot, baseFolder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
             
            string ext = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(ext) && !string.IsNullOrEmpty(defaultExtension))
                fileName += defaultExtension;

            return Path.Combine(folderPath, fileName);
        }


        [HttpGet("DistrictDD")]
        public async Task<IActionResult> GetDropdown()
        {
            var districts = await _repository.GetDistrictDropdownAsync();
            return Ok(districts);
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
            string filePath = GetFilePath("WeatherReportFiles/TempFiles", dto.FileName);

            var result =   _repository.SaveFileAsync(dto, filePath);
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
                           ? "ಪ್ರಸ್ತುತ ಹವಾಮಾನ"
                           : "ಇಂದಿನ ಹವಾಮಾನ"
            });

            return Ok(result);
        }

        [HttpPost("DisplayWeeklyWeatherFile")]
        public IActionResult DisplayWeeklyWeatherFile([FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("FileName is required.");

            try
            {
                // Full path to the file on server
                string filePath = GetFilePath("WeatherReportFiles/TempFiles", fileName);
                 

                if (!System.IO.File.Exists(filePath))
                    return NotFound("File not found.");

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var base64Content = Convert.ToBase64String(fileBytes);

                // Return as JSON with Base64 content
                return Ok(new
                {
                    FileName = fileName,
                    ContentType = "application/pdf",
                    Base64Content = base64Content
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("DisplayWeeklyWeatherFileOld")]
        public async Task<IActionResult> DisplayWeeklyWeatherFileold(string FileName)
        {
            if (string.IsNullOrEmpty(FileName))
                return BadRequest("FileName is required.");

            string filePath = GetFilePath("WeatherReportFiles/TempFiles", FileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");
             
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
             
            string contentType = GetContentType(filePath);
             
            string base64Content = Convert.ToBase64String(fileBytes);
            return Ok(base64Content);
            //return Ok(new
            //{
            //    FileName = Path.GetFileName(filePath),
            //    ContentType = contentType,
            //    Base64Content = base64Content
            //});
        }

        // Helper to get MIME type
        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
        {".txt", "text/plain"},
        {".pdf", "application/pdf"},
        {".doc", "application/msword"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".jpg", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".png", "image/png"},
        {".gif", "image/gif"},
        {".csv", "text/csv"}
        // Add more as needed
    };

            string ext = Path.GetExtension(path);
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }

        

    }
}
