using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.ViewModels.Crop;
using UHSB_Bagalkot.Service.ViewModels.Sections;

namespace UHSB_Bagalkot.WebApp.Controllers
{
    public class HorticulturehandbookwebController : Controller
    {
        private readonly ApiSettings _apiSettings;

        private readonly HttpClient _httpClient;

        public HorticulturehandbookwebController(IHttpClientFactory httpClientFactory, IConfiguration config, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _apiSettings = apiSettings.Value;
        }


        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.fileBaseUrl = _apiSettings.File_Url;

            return View();
        }

        [HttpGet]
        public IActionResult ForwardApiResponse(string action_name = "")
        {
            var apiUrl = $"{_apiSettings.Base_Url}/{action_name}";

            // Read token from Session
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            }

            var response = _httpClient.GetAsync(apiUrl).Result;

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"Error calling API: {apiUrl}");
            }

            var result = response.Content.ReadAsStringAsync().Result;
            var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";

            return Content(result, contentType);
        }

        [HttpPost] 
        public async Task<IActionResult> SaveOrEditCrop([FromForm] CropGridVM model, [FromForm] List<IFormFile> ImageFiles)
        {
         

            var uploadPaths = new List<string>();
            CropDetailsVM jsonobj = new CropDetailsVM();
            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                var uploadPath = _apiSettings.UploadSettings.TempFilesPath+ "\\Crops";

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                if (model.filename != null)
                {
                    var existingFilePath = Path.Combine(uploadPath, model.filename);
                    if (System.IO.File.Exists(existingFilePath))
                    {
                        System.IO.File.Delete(existingFilePath);
                    }
                }
                foreach (var file in ImageFiles)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = $"{model.Name}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    fileName = fileName.Replace(" ", "_");
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    uploadPaths.Add(fileName);
                }

                jsonobj.ImageUrl = uploadPaths.FirstOrDefault();
                jsonobj.CategoryId = model.CategoryId;
                jsonobj.Name = model.Name; 
                jsonobj.CropId = model.CropId;
            }

            var json = System.Text.Json.JsonSerializer.Serialize(jsonobj);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var apiUrl = $"{_apiSettings.Base_Url}/HorticultureHandbook/saveOrEditCrops";
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            }
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Saved successfully!";
                return Json(new { success = false, message = "Item saved successfully!." });

            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Error saving data: " + error);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrEditItem([FromForm] ItemDetailsVM model, List<IFormFile> ImageFiles)
        {
            var uploadPaths = new List<string>();
            var jsonObj = new RequestItemDetailsVM();

            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                var uploadPath = Path.Combine(_apiSettings.UploadSettings.TempFilesPath, "CropsItems");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                if(model.filename != null)
                {
                    var existingFilePath = Path.Combine(uploadPath, model.filename);
                    if (System.IO.File.Exists(existingFilePath))
                    {
                        System.IO.File.Delete(existingFilePath);
                    }
                }

                foreach (var file in ImageFiles)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = $"{model.Name}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    fileName = fileName.Replace(" ", "_");
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    uploadPaths.Add(fileName);
                }

                jsonObj.ImageUrl = uploadPaths.FirstOrDefault();
            }

            jsonObj.ItemId = model.ItemId;
            jsonObj.CategoryId = model.CategoryId;
            jsonObj.CropId = model.CropId;
            jsonObj.SectionId = model.SectionId;
            jsonObj.Name = model.Name;
            

            var json = System.Text.Json.JsonSerializer.Serialize(jsonObj);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var apiUrl = $"{_apiSettings.Base_Url}/HorticultureHandbook/SaveOrEditItemDetails";

            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            }

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Item saved successfully!";
                return Json(new { success = false, message = "Item saved successfully!." });
                 
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Error saving item: {error}");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrEditSection([FromForm] SectionsGridVM model, List<IFormFile> ImageFiles)
        {
            var uploadPaths = new List<string>();
            var jsonObj = new RequestSectionDetailsVM();

            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                var uploadPath = Path.Combine(_apiSettings.UploadSettings.TempFilesPath, "Sections");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                if (model.filename != null)
                {
                    var existingFilePath = Path.Combine(uploadPath, model.filename);
                    if (System.IO.File.Exists(existingFilePath))
                    {
                        System.IO.File.Delete(existingFilePath);
                    }
                }
                foreach (var file in ImageFiles)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = $"{model.Name}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    fileName = fileName.Replace(" ", "_");
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    uploadPaths.Add(fileName);
                }

                jsonObj.ImageUrl = uploadPaths.FirstOrDefault();
            }

            jsonObj.SectionId = model.SectionId;   
            jsonObj.Name = model.Name;

            var json = System.Text.Json.JsonSerializer.Serialize(jsonObj);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var apiUrl = $"{_apiSettings.Base_Url}/HorticultureHandbook/SaveOrEditSectionDetails";

            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            }

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Item saved successfully!";
                return Json(new { success = false, message = "Item saved successfully!." });
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Error saving item: {error}");
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> SaveOrEditCategory([FromForm] CategoryGridVM model, List<IFormFile> ImageFiles)
        {
            var uploadPaths = new List<string>();
            var jsonObj = new RequestCategoryDetailsVM();

            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                var uploadPath = Path.Combine(_apiSettings.UploadSettings.TempFilesPath, "Category");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                if (model.filename != null)
                {
                    var existingFilePath = Path.Combine(uploadPath, model.filename);
                    if (System.IO.File.Exists(existingFilePath))
                    {
                        System.IO.File.Delete(existingFilePath);
                    }
                }
                foreach (var file in ImageFiles)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = $"{model.Name}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    fileName = fileName.Replace(" ", "_");
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    uploadPaths.Add(fileName);
                }

                jsonObj.ImageUrl = uploadPaths.FirstOrDefault();
            }

            jsonObj.CategoryId = model.CategoryId;
            jsonObj.Name = model.Name;

            var json = System.Text.Json.JsonSerializer.Serialize(jsonObj);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var apiUrl = $"{_apiSettings.Base_Url}/HorticultureHandbook/SaveOrEditCategoryDetails";

            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            }

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Item saved successfully!";
                return Json(new { success = false, message = "Item saved successfully!." });
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Error saving item: {error}");
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteItems([FromBody] DeleteItemVM delmodel)
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(delmodel);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var apiUrl = $"{_apiSettings.Base_Url}/HorticultureHandbook/DeleteallpageItems";

                var accessToken = HttpContext.Session.GetString("AccessToken");
                if (!string.IsNullOrEmpty(accessToken))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                }

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Delete successfully!";
                    var result = response.Content.ReadAsStringAsync();
                    return Json(new { result.Result });
                }

                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Error deleting item: {error}");
                return Json(new { success = false, message = "Failed to delete item." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred: " + ex.Message });
            }
        }



    }
}
