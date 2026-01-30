using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.ViewModels;
using UHSB_Bagalkot.Service.ViewModels.AvailabilityTools;
using UHSB_Bagalkot.Service.ViewModels.Product;
using static UHSB_Bagalkot.Service.Common.GridEnum;

namespace UHSB_Bagalkot.WebApp.Controllers
{
    [Authorize]
    public class AvailabilityToolsWebController : Controller
    {
        private readonly ApiSettings _apiSettings;

        private readonly HttpClient _httpClient;

        public AvailabilityToolsWebController(IHttpClientFactory httpClientFactory, IConfiguration config, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _apiSettings = apiSettings.Value;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CentersAvailabilityDetails()
        {
            var model = new ProductsVM
            {
                Centerstype = new List<System.Web.Mvc.SelectListItem>(),
                Centervariatestype = new List<System.Web.Mvc.SelectListItem>(),

                ProductVarietyItems = new List<ProductVarietyVM>
                {
                    new ProductVarietyVM
                    {
                        VarietiesId = 0,
                        ProductId = 0,
                        CenterId = 0,

                        VarietyNameEng = string.Empty,
                        VarietyNameKnd = string.Empty,

                        StockKeepingUnit = string.Empty,
                        Barcode = string.Empty,

                        UnitId = 0,
                        Quantity = null,

                        Mrpprice = 0,
                        SellingPrice = 0,

                        StockQty = 0,
                        MinStockQty = null,

                        IsActive = true,
                        Filepath = string.Empty,
                        Remarks = string.Empty,

                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    }
                }
            };

            ViewBag.fileBaseUrl = _apiSettings.File_Url;
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> CentersAvailabilityDetails(ProductsVM model)
        {
            // ---------------- VALIDATION ----------------
            if (model.ProductVarietyItems == null || !model.ProductVarietyItems.Any())
            {
                ModelState.AddModelError("", "At least one product variety is required.");
            }

            for (int i = 0; i < model.ProductVarietyItems.Count; i++)
            {
                var item = model.ProductVarietyItems[i];
                int rowNumber = i + 1;

                if (string.IsNullOrWhiteSpace(item.VarietyNameEng))
                    ModelState.AddModelError($"ProductVarietyItems[{i}].VarietyNameEng",
                        $"Row {rowNumber}: Variety Name (English) is required.");

                if (string.IsNullOrWhiteSpace(item.VarietyNameKnd))
                    ModelState.AddModelError($"ProductVarietyItems[{i}].VarietyNameKnd",
                        $"Row {rowNumber}: Variety Name (Kannada) is required.");

                if (item.UnitId <= 0)
                    ModelState.AddModelError($"ProductVarietyItems[{i}].UnitId",
                        $"Row {rowNumber}: Unit is required.");

                //if (item.Mrpprice <= 0)
                //    ModelState.AddModelError($"ProductVarietyItems[{i}].Mrpprice",
                //        $"Row {rowNumber}: MRP must be greater than 0.");

                if (item.SellingPrice <= 0)
                    ModelState.AddModelError($"ProductVarietyItems[{i}].SellingPrice",
                        $"Row {rowNumber}: Selling Price must be greater than 0.");

                //if (item.SellingPrice > item.Mrpprice)
                //    ModelState.AddModelError($"ProductVarietyItems[{i}].SellingPrice",
                //        $"Row {rowNumber}: Selling Price cannot be greater than MRP.");

                if (item.StockQty < 0)
                    ModelState.AddModelError($"ProductVarietyItems[{i}].StockQty",
                        $"Row {rowNumber}: Stock Quantity cannot be negative.");

                //if (item.MinStockQty <= 0)
                //    ModelState.AddModelError($"ProductVarietyItems[{i}].MinStockQty",
                //        $"Row {rowNumber}: Minimum Stock Quantity must be greater than 0.");
            }

            // If validation fails, return JSON errors for AJAX
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                });
            }

            // ---------------- FILE UPLOAD ----------------
            var uploadPaths = new List<string>();

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadPath = Path.Combine(_apiSettings.UploadSettings.TempFilesPath, "Products");
                Directory.CreateDirectory(uploadPath);

                var extension = Path.GetExtension(model.ImageFile.FileName);
                var fileName = $"{model.ProductNameKnd.Trim().Replace(" ", "_")}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                model.Filepath = fileName;
                model.ImageFile = null;
            }

            // ---------------- VARIETY IMAGES ----------------
            var uploadPathVariets = Path.Combine(_apiSettings.UploadSettings.TempFilesPath, "ProductswithVariets");
            Directory.CreateDirectory(uploadPathVariets);

            foreach (var variety in model.ProductVarietyItems)
            {
                if (variety.ImageFile != null && variety.ImageFile.Length > 0)
                {
                    var varietyFileName = Guid.NewGuid() + Path.GetExtension(variety.ImageFile.FileName);
                    var varietyPath = Path.Combine(uploadPathVariets, varietyFileName);

                    using (var stream = new FileStream(varietyPath, FileMode.Create))
                    {
                        await variety.ImageFile.CopyToAsync(stream);
                    }

                    variety.Filepath = varietyFileName;
                    variety.ImageFile = null;

                }
            }

            // ---------------- SEND TO API ----------------
            var token = HttpContext.Session.GetString("AuthToken");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(model),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                $"{_apiSettings.Base_Url}/AvailabilityTools/SaveOrEditProdectDetails",
                jsonContent
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = "Failed : " + error });
            }

            var json = await response.Content.ReadAsStringAsync();
            var responsetext = System.Text.Json.JsonSerializer.Deserialize<saveresponse>(json);

            if (responsetext != null && responsetext.success)
            {
                return Json(new { success = true, message = "Data saved successfully" });
            }
            else
            {
                return Json(new { success = false, message = responsetext?.message ?? "Failed to save data" });
            }
        }


        [HttpPost]
        public IActionResult CentersAvailabilityDetailsOLD(ProductsVM model)
        {

            if (model.ProductVarietyItems == null || !model.ProductVarietyItems.Any())
            {
                ModelState.AddModelError("", "At least one product variety is required.");
                return View(model);
            }
            for (int i = 0; i < model.ProductVarietyItems.Count; i++)
            {
                var item = model.ProductVarietyItems[i];
                int rowNumber = i + 1; // for user-friendly display

                if (string.IsNullOrWhiteSpace(item.VarietyNameEng))
                    ModelState.AddModelError($"ProductVarietyItems[{i}].VarietyNameEng",
                        $"Row {rowNumber}: Variety Name (English) is required.");

                if (string.IsNullOrWhiteSpace(item.VarietyNameKnd))
                    ModelState.AddModelError($"ProductVarietyItems[{i}].VarietyNameKnd",
                        $"Row {rowNumber}: Variety Name (Kannada) is required.");

                if (item.UnitId <= 0)
                    ModelState.AddModelError($"ProductVarietyItems[{i}].UnitId",
                        $"Row {rowNumber}: Unit is required.");

                //if (item.Mrpprice <= 0)
                //    ModelState.AddModelError($"ProductVarietyItems[{i}].Mrpprice",
                //        $"Row {rowNumber}: MRP must be greater than 0.");

                if (item.SellingPrice <= 0)
                    ModelState.AddModelError($"ProductVarietyItems[{i}].SellingPrice",
                        $"Row {rowNumber}: Selling Price must be greater than 0.");

                //if (item.SellingPrice > item.Mrpprice)
                //    ModelState.AddModelError($"ProductVarietyItems[{i}].SellingPrice",
                //        $"Row {rowNumber}: Selling Price cannot be greater than MRP.");

                if (item.StockQty < 0)
                    ModelState.AddModelError($"ProductVarietyItems[{i}].StockQty",
                        $"Row {rowNumber}: Stock Quantity cannot be negative.");

                //if (item.MinStockQty <= 0)
                //    ModelState.AddModelError($"ProductVarietyItems[{i}].MinStockQty",
                //        $"Row {rowNumber}: Minimum Stock Quantity must be greater than 0.");

                if (item.Filepath != null && item.Filepath.Length > 0)
                {
                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(item.Filepath))
                    {
                        ModelState.AddModelError($"ProductVarietyItems[{i}].Filepath",
                            $"Row {rowNumber}: Only JPG, PNG, or GIF images are allowed.");
                    }
                }
            }


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = HttpContext.Session.GetString("AuthToken");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var uploadPaths = new List<string>();

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadPath = _apiSettings.UploadSettings.TempFilesPath + "\\Products";

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                if (model.Filepath != null)
                {
                    var existingFilePath = Path.Combine(uploadPath, model.Filepath);
                    if (System.IO.File.Exists(existingFilePath))
                    {
                        System.IO.File.Delete(existingFilePath);
                    }
                }
                //foreach (var file in model.ImageFiles)
                //{
                var extension = Path.GetExtension(model.ImageFile.FileName);
                var fileName = $"{model.ProductNameKnd.Trim()}{extension}";
                fileName = fileName.Replace(" ", "_");
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyToAsync(stream);
                }

                uploadPaths.Add(fileName);
                //}



                model.Filepath = uploadPaths.FirstOrDefault();
            }

            // ================= VARIETY IMAGES =================
            var uploadPathVariets = _apiSettings.UploadSettings.TempFilesPath + "\\ProductswithVariets";
            if (!Directory.Exists(uploadPathVariets))
                Directory.CreateDirectory(uploadPathVariets);
            foreach (var variety in model.ProductVarietyItems)
            {
                if (variety.ImageFile != null && variety.ImageFile.Length > 0)
                {
                    var varietyFileName = Guid.NewGuid() + Path.GetExtension(variety.ImageFile.FileName);
                    var varietyPath = Path.Combine(uploadPathVariets, varietyFileName);

                    using (var stream = new FileStream(varietyPath, FileMode.Create))
                    {
                        variety.ImageFile.CopyToAsync(stream);
                    }
                    variety.Filepath = varietyFileName;
                }
            }


            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(model),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = _httpClient.PostAsync(
                $"{_apiSettings.Base_Url}/AvailabilityTools/SaveOrEditProdectDetails",
                jsonContent
            ).Result;

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync();

                var responsetext = System.Text.Json.JsonSerializer.Deserialize<saveresponse>(json.Result);

                if (responsetext != null)
                {
                    if (responsetext.success)
                    {
                        TempData["Success"] = "Data saved successfully";
                        return RedirectToAction(nameof(CentersAvailabilityDetails));
                    }
                    else
                    {
                        TempData["Error"] = responsetext.message;
                        return RedirectToAction(nameof(CentersAvailabilityDetails));
                    }
                }
                TempData["Error"] = "Failed to save data";
                return RedirectToAction(nameof(CentersAvailabilityDetails));
            }

            TempData["Error"] = "Failed to save data";
            return RedirectToAction(nameof(CentersAvailabilityDetails));
        }


        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetbyIdProdect(int identifier = 0)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Unauthorized. Please login." });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Unauthorized. Please login." });
                }


                // Make GET request
                var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/AvailabilityTools/GetbyIdProdect?identifier={identifier}");

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = $"Error fetching content: {response.StatusCode}" });
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ProductsVM>(content);

                if (result == null)
                {
                    return Json(new { success = true, data = new List<ProductsVM>() });
                }

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return JSON
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost] 
        public async Task<IActionResult> GetProductVarieties(int productid = 0)
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

                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Unauthorized. Please login." });
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                //int centerid = 0, int districtid = 0, int pagetype = 0
                var query = new Dictionary<string, string>
                {
                    ["productid"] = productid.ToString(),
                };

                // Convert to query string
                var queryString = string.Join("&", query
                    .Where(kv => !string.IsNullOrEmpty(kv.Value))
                    .Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));

                // Make GET request
                var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/AvailabilityTools/GetProductVarieties?{queryString}");

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = $"Error fetching content: {response.StatusCode}" });
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<List<ProductVariety_SP_VM>>>(content);

                if (result?.Data == null || !result.Data.Any())
                {
                    return Json(new { success = false, data = new List<ProductVariety_SP_VM>() });
                }

                return Json(new { success = true, data = result.Data });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return JSON
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetGridContentV1(int currentPage = 1, int pageSize = 10, GridEnum.UserMasterColumns orderBy = GridEnum.UserMasterColumns.UserName, bool isDescending = false, string filterDetails = null, string externalFilter = null, int centerid = 0, int districtid = 0, int pagetype = 0)
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

                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Unauthorized. Please login." });
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                //int centerid = 0, int districtid = 0, int pagetype = 0
                var query = new Dictionary<string, string>
                {
                    ["currentPage"] = currentPage.ToString(),
                    ["pageSize"] = pageSize.ToString(),
                    ["orderBy"] = orderBy.ToString(),
                    ["isDescending"] = isDescending.ToString(),
                    ["filterDetails"] = filterDetails ?? string.Empty,
                    ["externalFilter"] = externalFilter ?? string.Empty,
                    ["centerid"] = centerid.ToString(),
                    ["districtid"] = districtid.ToString(),
                    ["pagetype"] = pagetype.ToString(),
                };

                // Convert to query string
                var queryString = string.Join("&", query
                    .Where(kv => !string.IsNullOrEmpty(kv.Value))
                    .Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));

                // Make GET request
                var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/AvailabilityTools/getgridcontentproducts?{queryString}");

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = $"Error fetching content: {response.StatusCode}" });
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<GenericGridModel<Product_SP_VM>>>(content);

                if (result?.Data == null || !result.Data.ItemDetails.Any())
                {
                    return Json(new { success = true, data = new List<Product_SP_VM>() });
                }

                return Json(new { success = true, data = result.Data });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return JSON
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> EuipmentIndex()
        {
            ViewBag.fileBaseUrl = _apiSettings.File_Url;
            var model = new ProductsVM
            {
                Centerstype = new List<System.Web.Mvc.SelectListItem>(),
                Centervariatestype = new List<System.Web.Mvc.SelectListItem>()
            };
            ViewBag.fileBaseUrl = _apiSettings.File_Url;
            return View(model);
        }

        public IActionResult VarietiesIndex()
        {
            ViewBag.fileBaseUrl = _apiSettings.File_Url;
            return View();
        }


    }
}
