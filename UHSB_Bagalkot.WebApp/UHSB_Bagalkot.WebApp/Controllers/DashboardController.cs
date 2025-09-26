using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Helpers;
using System.Web.WebPages;
using UHSB_Bagalkot.Service.ViewModels.AdminDashboard;
using UHSB_Bagalkot.WebService.AppSettings;
using UHSB_Bagalkot.WebService.ViewModels;
using UHSB_Bagalkot.WebService.ViewModels.ManageAdminDashboard;
using UHSB_MasterService.Common;
using static System.Collections.Specialized.BitVector32;

namespace UHSB_Bagalkot.WebApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApiSettings _apiSettings;

        private readonly HttpClient _httpClient;

        public DashboardController(IHttpClientFactory httpClientFactory, IConfiguration config, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiSettings = apiSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> AdminHome(int districtId = 1)
        {
            var vm = new AdminDashboardVM();

            // ✅ Call Summary API
            var summaryRes = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/Dashboard/summary");
            if (summaryRes.IsSuccessStatusCode)
            {
                var json = await summaryRes.Content.ReadAsStringAsync();
                vm.Summary = JsonConvert.DeserializeObject<SummaryVM>(json);
            }

            // ✅ Call Farmers by Village API
            var farmersRes = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/Dashboard/farmers-by-village");
            if (farmersRes.IsSuccessStatusCode)
            {
                var json = await farmersRes.Content.ReadAsStringAsync();
                vm.FarmersByVillage = JsonConvert.DeserializeObject<List<FarmersByVillageVM>>(json);
            }

            // ✅ Call Weekly Weather API
            var weatherRes = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/Dashboard/weekly-weather/{districtId}");
            if (!weatherRes.IsSuccessStatusCode)
            {
                var json = await weatherRes.Content.ReadAsStringAsync();
                vm.WeeklyWeather = JsonConvert.DeserializeObject<WeeklyWeatherVM>(json);
            }

            return View(vm);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageAdminDashboard()
        {

            return View();
        }

        public IActionResult CropManage()
        {
            var model = new CropContentWithImageVM
            {
                Categories = new List<System.Web.Mvc.SelectListItem>(),
                Crops = new List<System.Web.Mvc.SelectListItem>(),
                Sections = new List<System.Web.Mvc.SelectListItem>(),
                SubSections = new List<System.Web.Mvc.SelectListItem>(),
                Items = new List<System.Web.Mvc.SelectListItem>()
            };
            model.ItemImages = new List<CropContentItems>
            {
                new CropContentItems
                {
                    ItemId = 0,
                    ImageFile = null,
                    Description = ""
                }
            };
            return View(model);
        }



        [HttpGet]
        public async Task<JsonResult> GetCategories()
        {

            var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url + "/Dashboard/Category"}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<List<DropdownVM>>>(json);
            return Json(result.Data);
        }



        [HttpGet]
        public async Task<JsonResult> GetCrops(int catId)
        {
            var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url + "/Dashboard/Crops/" + catId}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<List<DropdownVM>>>(json);
            return Json(result.Data);
        }



        [HttpGet]
        public async Task<JsonResult> GetSections(int cropId)
        {
            var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url + "/Dashboard/section/" + cropId}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<List<DropdownVM>>>(json);
            return Json(result.Data);
        }


        [HttpGet]
        public async Task<JsonResult> GetSubSections(int sectId)
        {
            var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url + "/Dashboard/SubSection/" + sectId}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<List<DropdownVM>>>(json);
            return Json(result.Data);
        }

        [HttpGet]
        public async Task<JsonResult> GetItems(int subSectId)
        {
            var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url + "/Dashboard/items/" + subSectId}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<List<DropdownVM>>>(json);
            return Json(result.Data);
        }


        //Get CropManage
        [HttpGet]
        public async Task<IActionResult> GetgridItems(int subSectId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Unauthorized. Please login." });
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/Dashboard/GetGridItems/{subSectId}");

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = $"Error fetching content: {response.StatusCode}" });
                }

                var content = await response.Content.ReadAsStringAsync();

                // Deserialize API response
                var result = JsonConvert.DeserializeObject<ApiResponse<List<CropContentItems>>>(content);

                if (result?.Data == null || !result.Data.Any())
                {
                    return Json(new { success = true, data = new List<CropContentItems>() });
                }

                return Json(new { success = true, data = result.Data });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return JSON
                return Json(new { success = false, message = ex.Message });
            }
        }


        //Save
        [HttpPost]
        public async Task<IActionResult> CropManage(CropContentWithImageVM model, [FromServices] IWebHostEnvironment env)
        {
            List<UhsbItemImageVM> jsonModel = new List<UhsbItemImageVM>();
            foreach (var item in model.ItemImages)
            {
                UhsbItemImageVM obj = new UhsbItemImageVM();
                if (item.ImageFile != null && item.ImageFile.Length > 0)
                {
                    // Folder path: wwwroot/InwardsInvoices/TempFiles (you can change it)
                    //var uploadPath = Path.Combine(env.WebRootPath, "InwardsInvoices", "TempFiles");
                  
                    var uploadPath = _apiSettings.UploadSettings.TempFilesPath;

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    // Generate unique filename
                    var extension = Path.GetExtension(item.ImageFile.FileName);
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await item.ImageFile.CopyToAsync(stream);
                    }

                    // Store relative path to send in JSON
                    obj.ImageUrl = Path.Combine("InwardsInvoices", "TempFiles", fileName).Replace("\\", "/");
                    obj.ItemId=item.ItemId??0;
                    obj.Description=item.Description;
                    // Optionally clear the IFormFile to reduce JSON size
                    jsonModel.Add(obj);
                }
            }

            // Serialize model to JSON
            var json = System.Text.Json.JsonSerializer.Serialize(jsonModel);

            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // API URL
            var apiUrl = $"{_apiSettings.Base_Url}/Dashboard/SaveCropContent";

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Saved successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "Error saving data: " + error);
                return View(model);
            }
        }
         
        //File Uploads
        [HttpPost]
        public IActionResult Index(CropContentWithImageVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                return View("_errorView", errors);
            }

            string tempFiledet = "";
            string invoiceDateTaxable = "";

            //if (!string.IsNullOrEmpty(model.TempFilePath))
            //{

            //    if (!string.IsNullOrEmpty(model.FilePath))
            //    {
            //        // Replace Server.MapPath with Path.Combine
            //        var detail = CommonHelper.GetFilePathFTP(
            //            model.TempFilePath,
            //            model.FilePath,
            //            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "InwardsInvoices", "TempFiles"),
            //            "Taxable"
            //        );

            //        model.FilePath = detail[0];
            //        tempFiledet = detail[1];
            //    }
            //}

            short branchId = 0;
            if (HttpContext.Session != null && HttpContext.Session.GetString("BranchId") != null)
            {
                branchId = Convert.ToInt16(HttpContext.Session.GetString("BranchId"));
            }

            var result =true ;//_db.AddEditRecord(model, currentUserId, currentUserRole, branchId);

            if (result)
            {
                //if (!string.IsNullOrEmpty(model.FilePath) && !string.IsNullOrEmpty(model.TempFilePath))
                //{
                //    CommonHelper.SaveFileFTP(
                //        tempFiledet,
                //        model.FilePath,
                //        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "InwardsInvoices"),
                //        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "InwardsInvoices", "TempFiles"),
                //        "Taxable"
                //    );
                //}

                return Content("true");
            }
            else
            {
                return Content("false");
            }
        }


        public ActionResult GetSearchView(GridEnum.ReferenceEnum filterBy)
        {
            var model = EnumHelper.GetOutputEnum(filterBy);
            model.filterBy = (int)filterBy;
            return View("_SearchFilterView", model);

        }
        [HttpPost]
        public IActionResult UploadFiles(IFormFile image1, string filedate, [FromServices] IWebHostEnvironment env)
        {
            if (image1 != null)
            {
                var uploadPath = Path.Combine(env.WebRootPath, "InwardsInvoices", "TempFiles");

                var filePath = CommonHelper.UploadFilesInwardFTP(image1, filedate, uploadPath);

                if (filePath == null)
                {
                    return Json(new { FilePath = "false" });
                }
                else if (filePath == "filelength")
                {
                    return Json(new { FilePath = "filelength" });
                }
                else if (filePath == "wrongfileformat")
                {
                    return Json(new { FilePath = "wrongfileformat" });
                }
                else
                {
                    return Json(new { FilePath = filePath });
                }
            }

            return Json(new { FilePath = "false" });
        }


        [HttpPost]
        public IActionResult ViewFiles(List<IFormFile> files)
        {
            string finalpath = string.Empty;
            string finalFilePath = string.Empty;

            if (files != null && files.Count > 0)
            {
                try
                {
                    foreach (var file in files)
                    {
                        string fname;

                        // File name handling (no IE check in .NET Core, FileName is safe)
                        fname = file.FileName;
                        finalFilePath = file.FileName;

                        var serverpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SaveInwardFiles");

                        if (!Directory.Exists(serverpath))
                        {
                            Directory.CreateDirectory(serverpath);
                        }

                        FileInfo[] fi = new FileInfo[0];
                        DirectoryInfo di = null;
                        if (Directory.Exists(serverpath))
                        {
                            di = new DirectoryInfo(serverpath);
                            if (di.Exists)
                            {
                                fi = di.GetFiles().ToArray();
                            }
                            if (fi.Length > 0)
                            {
                                foreach (FileInfo file1 in fi)
                                {
                                    file1.Delete();
                                }
                            }
                        }

                        // Save file
                        var fullPath = Path.Combine(serverpath, fname);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        finalpath = Path.Combine("SaveInwardFiles", finalFilePath);
                    }

                    return Json(finalpath);
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }


        public IActionResult DownloadFile(string FilePath)
        {
            if (string.IsNullOrEmpty(FilePath))
                return BadRequest("File path is required.");

            string filePath = null;
            string ExtName = null;

            var serverPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "InwardsInvoices");

            // Your helper method (unchanged)
            CommonHelper.DownloadFileInwardFTP(FilePath, ref filePath, ref ExtName, serverPath);

            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string contentType = ExtName ?? "application/octet-stream";
                string downloadFileName = Path.GetFileName(FilePath);

                return File(fileBytes, contentType, downloadFileName);
            }
            else
            {
                throw new FileNotFoundException("File not found: " + FilePath);
            }
        }


    }

}
 