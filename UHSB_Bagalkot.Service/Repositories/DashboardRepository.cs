using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.Dto;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels;
using UHSB_Bagalkot.Service.ViewModels.AdminDashboard;  

namespace UHSB_Bagalkot.Service.Repositories
{
    public class DashboardRepository : CommonConnection, IDashboardRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DashboardRepository(Uhsb2025uatContext context) : base(context)
        {

        } 

        public string GetConnectionString()
        {
            return _context.Database.GetDbConnection().ConnectionString;
        }
        public async Task<DashboardSummaryVM> GetSummaryAsync()
        {
            return new DashboardSummaryVM
            {
                TotalUsers = await _context.UserMasters.CountAsync(),
                ActiveUsers = await _context.UserMasters.CountAsync(u => u.IsActive),
                Farmers = await _context.FarmersProfiles.CountAsync(),
                Categories = await _context.UhsbCategories.CountAsync(),
                Crops = await _context.UhsbCrops.CountAsync(),
                WeatherFiles = await _context.UhsbWeatherCastFileDetails.CountAsync()
            };
        }

        public async Task<IEnumerable<object>> GetFarmersByVillageAsync()
        {
            return await _context.FarmersProfiles
                .GroupBy(f => f.Village)
                .Select(g => new { Village = g.Key, Count = g.Count() })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetWeeklyWeatherAsync(int districtId)
        {
            return await _context.UhsbWeatherCastFileDetails
                .FromSqlRaw("EXEC sp_GetWeeklyWeatherRecords @DistrictId",
                    new SqlParameter("@DistrictId", districtId))
                .ToListAsync();
        }


        #region Crop Manage
        public async Task<IEnumerable<DropdownVM>> CategoryDD()
        {
            return await _context.UhsbCategories
                .Select(c => new DropdownVM { Id = c.CategoryId, Name = c.Name })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownVM>> CropsDD(int categoryId)
        {
            return await _context.UhsbCrops
                .Where(c => c.CategoryId == categoryId)
                .Select(c => new DropdownVM { Id = c.CropId, Name = c.Name })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownVM>> SectionDD(int cropId)
        {
            return await _context.UhsbSections
                .Select(s => new DropdownVM { Id = s.SectionId, Name = s.Name })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownVM>> SubSectionDD(int sectionId)
        {
            return await _context.UhsbSubSections
                .Where(ss => ss.SectionId == sectionId)
                .Select(ss => new DropdownVM { Id = ss.SubSectionId, Name = ss.Name })
                .ToListAsync();
        }

        public async Task<IEnumerable<DropdownVM>> ItemDeailsDD(int sectionId,int cropId)
        {
            //var details = await (
            //    from item in _context.UhsbItemDeails
            //    where !_context.UhsbItemImages.Any(img => img.ItemId == item.ItemId)
            //          && item.SectionId == sectionId
            //    select new DropdownVM
            //    {
            //        Id = item.ItemId,
            //        Name = item.Name
            //    }
            //).ToListAsync();

            var result = await (from items in   _context.UhsbItemDeails 
                                where items.CropId == cropId && items.SectionId == sectionId
                                select new DropdownVM
                                {
                                    Id = items.ItemId,
                                    Name = items.Name,
                                }).ToListAsync();
            var result1 = await (from map in _context.UhsbSectionsMappings
                                join section in _context.UhsbSections
                                    on map.SectionId equals section.SectionId
                                join items in _context.UhsbItemDeails
                                    on map.SectionMapId equals items.SectionMapId
                                where map.CropId == cropId && section.SectionId == sectionId
                                select new DropdownVM
                                {
                                    Id = items.ItemId,
                                    Name = items.Name,
                                }).ToListAsync();

            return result;
        }
        //public async Task<IEnumerable<UhsbItemImageVM>> GetgridItems(int subSectionId)
        //{
        //    var relativePath = "";  

        //    var data = await (from d in _context.UhsbItemDeails
        //                      join img in _context.UhsbItemImages
        //                          on d.ItemId equals img.ItemId
        //                      where d.SubSectionId == subSectionId
        //                      select new UhsbItemImageVM
        //                      {
        //                          ImageId = img.ImageId,
        //                          ItemId = d.ItemId,
        //                          ImageUrl = relativePath + (img.ImageUrl ?? string.Empty).Replace("\\", "/"),
        //                          Description = img.Description
        //                      }).ToListAsync();
        //    return data;
        //}

        public async Task<GenericGridModel<UhsbItemImageVM>> GetGridItemsV2(int currentPage = 1, int pageSize = 10,  GridEnum.FTPDocumentsLogs orderBy = GridEnum.FTPDocumentsLogs.BranchName,
            bool isDescending = false,  string filterDetails = null, string externalFilter = null, 
            int subSectId = 0,int cropid=0,int categoryid=0)
        {
            var relativePath = "";

            // Base query

            //IQueryable<UhsbItemImageVM> queryold = from map in _context.UhsbSectionsMappings
            //                                    join section in _context.UhsbSections
            //                on map.SectionId equals section.SectionId
            //            join item in _context.UhsbItemDeails
            //                on map.SectionMapId equals item.SectionMapId
            //            join img in _context.UhsbItemImages
            //                on item.ItemId equals img.ItemId
            //            where item.CropId == cropid && item.SectionId == subSectId
            //                                    select new UhsbItemImageVM
            //            {
            //                ImageId = img.ImageId,
            //                ItemId = item.ItemId,
            //                ImageUrl = relativePath + (img.ImageUrl ?? string.Empty).Replace("\\", "/"),
            //                Description = img.Description,
            //                ItemName = item.Name
            //            };
            var result = new GenericGridModel<UhsbItemImageVM>();
            var items = new List<UhsbItemImageVM>();
      
            string connectionString = GetConnectionString();

            using SqlConnection con = new(connectionString);
            using SqlCommand cmd = new("SP_GetUhsb_FinalContent", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CategoryId", categoryid);
            cmd.Parameters.AddWithValue("@CropId", cropid);
            cmd.Parameters.AddWithValue("@SectionId", subSectId);

            await con.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new UhsbItemImageVM
                {
                    ImageId = reader["ImageId"] as int?,
                    ItemId = reader["ItemId"] as int?,
                    ImageUrl = relativePath + (reader["ImageUrl"]?.ToString() ?? string.Empty),
                    Description = reader["Description"]?.ToString() ?? string.Empty,
                    ItemName = reader["ItemName"]?.ToString() ?? string.Empty,
                    CategoryName = reader["CategoryName"]?.ToString() ?? string.Empty,
                    CategoryId = reader["CategoryId"] as int?,
                    CropName = reader["CropName"]?.ToString() ?? string.Empty,
                    CropId = reader["CropId"] as int?,
                    SectionName = reader["SectionName"]?.ToString() ?? string.Empty,
                    SectionId = reader["SectionId"] as int?
                });
            }

            // Convert to IQueryable for filtering
            var query = items.AsQueryable();

            //IQueryable<UhsbItemImageVM> query =
            //                             from item in _context.UhsbItemDeails
            //                             join img in _context.UhsbItemImages
            //                                 on item.ItemId equals img.ItemId
            //                             //where item.CropId == cropid && item.SectionId == subSectId
            //                             select new UhsbItemImageVM
            //                             {
            //                                 ImageId = img.ImageId,
            //                                 ItemId = item.ItemId,
            //                                 ImageUrl = relativePath + (img.ImageUrl ?? string.Empty).Replace("\\", "/"),
            //                                 Description = img.Description,
            //                                 ItemName = item.Name,
            //                                 CategoryId=item.CategoryId,
            //                                 CropId=item.CropId,
            //                                 SectionId=item.SectionId,
            //                             };

            //filters 
            //if (categoryid > 0)
            //{
            //    query= query.Where(x=> x.CategoryId == cropid);
            //}else if(cropid > 0)
            //{
            //    query = query.Where(x => x.CropId == cropid);
            //}
            //else if (subSectId > 0)
            //{
            //    query = query.Where(x => x.SectionId == subSectId);

            //}



            // Apply filters if any
            List<GridFilterModel> filters = null;
            if (!string.IsNullOrEmpty(filterDetails))
            {
                filters = JsonConvert.DeserializeObject<List<GridFilterModel>>(filterDetails);
                if (filters != null && filters.Count > 0)
                {
                    foreach (var filter in filters)
                    {
                        Expression<Func<UhsbItemImageVM, bool>> predicate =
                            GetWherePrediction((GridEnum.FTPDocumentsLogs)filter.filterBy,
                                               (filter.filterTxt ?? "").Trim(),
                                               (GridEnum.FilterTypeEnum)filter.filterType);
                        if (predicate != null)
                            query = query.Where(predicate);
                    }
                }
            }

            // Total count before paging
            var totalCount =   query.Count();

            // Ordering (example, can extend for more fields)
            //query = orderBy switch
            //{
            //    GridEnum.FTPDocumentsLogs.BranchName => isDescending ? query.OrderByDescending(x => x.ItemId) : query.OrderBy(x => x.ItemId),
            //    _ => query
            //};

            // Paging
            var dataList =   query.Skip((currentPage - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToList();
             
            var data = new GenericGridModel<UhsbItemImageVM>
            {
                ItemDetails = dataList,
                TotalCount = totalCount,
                currentPage = currentPage,
                CanAdd = true,      
                CanEdit = true,
                CanDelete = true,
                CanViewSingle = true,
                CanViewMultiple = true
            };

            return data;
        }



        #region GetByIdImageConentDetails

        public async Task<UhsbItemImageVM> GetByIdImageConentDetails(int imagecontentid)
        { 

            var data = await (from img in _context.UhsbItemImages
                              join item in _context.UhsbItemDeails
                                  on img.ItemId equals item.ItemId
                              where img.ImageId == imagecontentid
                              select new UhsbItemImageVM
                              {
                                  ImageId = img.ImageId,
                                  ItemId = item.ItemId,
                                  ImageUrl = img.ImageUrl?? "",
                                  Description = img.Description??"",
                                  CropId = item.CropId,
                                  SectionId = item.SectionId,
                                  CategoryId= item.CategoryId,
                              }).FirstOrDefaultAsync();
            return data;
        }

        #endregion



        public Expression<Func<UhsbItemImageVM, bool>> GetWherePrediction(GridEnum.FTPDocumentsLogs filterBy, string filterTxt, GridEnum.FilterTypeEnum filterType)
        {
            Expression<Func<UhsbItemImageVM, bool>> predicate = null;

            if (!string.IsNullOrEmpty(filterTxt))
            {
                switch (filterBy)
                {
                    case GridEnum.FTPDocumentsLogs.BranchName:
                        switch (filterType)
                        {
                            case GridEnum.FilterTypeEnum.Equal:
                                predicate = x => x.Description.Equals(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.Contains:
                                predicate = x => x.Description.Contains(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.StartsWith:
                                predicate = x => x.Description.StartsWith(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.EndsWith:
                                predicate = x => x.Description.EndsWith(filterTxt);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            return predicate;
        }
        public Expression<Func<UserMasterVM, bool>> GetWherePrediction1(GridEnum.UserMasterColumns filterBy, string filterTxt, GridEnum.FilterTypeEnum filterType)
        {
            Expression<Func<UserMasterVM, bool>> predicate = null;

            if (!string.IsNullOrEmpty(filterTxt))
            {
                switch (filterBy)
                {
                    case GridEnum.UserMasterColumns.UserName:
                        switch (filterType)
                        {
                            case GridEnum.FilterTypeEnum.Equal:
                                predicate = x => x.UserName.Equals(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.Contains:
                                predicate = x => x.UserName.Contains(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.StartsWith:
                                predicate = x => x.UserName.StartsWith(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.EndsWith:
                                predicate = x => x.UserName.EndsWith(filterTxt);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            return predicate;
        }

        #region Usermaster 
        public async Task<GenericGridModel<UserMasterVM>> GetGridUsermasterV2(int currentPage = 1, int pageSize = 10, GridEnum.UserMasterColumns orderBy = GridEnum.UserMasterColumns.CreatedDate, bool isDescending = false, string filterDetails = null, string externalFilter = null)
        {
            var relativePath = "";
             
            IQueryable<UserMasterVM> query = from um in _context.UserMasters 
                                                select new UserMasterVM
                                                {
                                                    Id = um.Id,
                                                    UserName = um.UserName,
                                                    PhoneNumber = um.PhoneNumber,
                                                    IsActive = um.IsActive,
                                                    CreatedDate = um.CreatedAt,
                                                    Village = um.Village,
                                                    CreatedBy=um.CreatedBy,
                                                    ModifiedBy=um.ModifiedBy,
                                                    ModifiedDate=um.ModifiedDate,
                                                    DistrictsId=um.DistrictsId??0,
                                                    DistrictsName = _context.UhsbDistricts.Where(c => c.DistrictId == um.DistrictsId).Select(c => c.DistrictName).FirstOrDefault(),
                                                    RoleType=um.RoleType??0
                                                };

            List<GridFilterModel> filters = null;
            if (!string.IsNullOrEmpty(filterDetails))
            {
                filters = JsonConvert.DeserializeObject<List<GridFilterModel>>(filterDetails);
                if (filters != null && filters.Count > 0)
                {
                    foreach (var filter in filters)
                    {
                        Expression<Func<UserMasterVM, bool>> predicate =
                            GetWherePrediction1((GridEnum.UserMasterColumns)filter.filterBy,
                                               (filter.filterTxt ?? "").Trim(),
                                               (GridEnum.FilterTypeEnum)filter.filterType);
                        if (predicate != null)
                            query = query.Where(predicate);
                    }
                }
            }

            var totalCount = await query.CountAsync();


            var dataList = await query.Skip((currentPage - 1) * pageSize)
                                      .Take(pageSize).OrderByDescending(x => x.CreatedDate)
                                      .ToListAsync();

            var data = new GenericGridModel<UserMasterVM>
            {
                ItemDetails = dataList,
                TotalCount = totalCount,
                currentPage = currentPage,
                CanAdd = true,
                CanEdit = true,
                CanDelete = true,
                CanViewSingle = true,
                CanViewMultiple = true
            };

            return data;
        }

        #endregion

        //save
        public async Task<bool> SaveCropContentAsync(List<UhsbItemImageVM> model)
        {
            if (model == null) return false;

            foreach (var item in model)
            {
                var existingCrop = await _context.UhsbItemImages
                    .FirstOrDefaultAsync(x => x.ImageId == item.ImageId);

                // If record exists → update
                if (existingCrop != null)
                {
                    if (!string.IsNullOrEmpty(item.ImageUrl))
                    {
                        existingCrop.ImageUrl = item.ImageUrl;

                    }
                    existingCrop.ItemId = item.ItemId ?? 0;

                    existingCrop.Description = item.Description;
                    existingCrop.ModifiedBy = 1;
                    existingCrop.ModifiedDate = DateTime.Now;
                }
                else
                {
                    // If record not found → insert new (optional)
                    var newCrop = new UhsbItemImage
                    {
                        ItemId = item.ItemId ?? 0,
                        Description = item.Description,
                        ImageUrl = item.ImageUrl,
                        CreatedBy = 1,
                        ModifiedBy = 1,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    _context.UhsbItemImages.Add(newCrop);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion
    }
}
