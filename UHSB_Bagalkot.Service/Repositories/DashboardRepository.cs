using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public DashboardRepository(Uhsb2025Context context) : base(context)
        {

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
                .Where(s => s.CropId == cropId)
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

            var result = await (from map in _context.UHSB_SectionsMappings
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
            int subSectId = 0,int cropid=0)
        {
            var relativePath = "";

            // Base query
            //IQueryable<UhsbItemImageVM> query1 = from d in _context.UhsbItemDeails
            //                                    join img in _context.UhsbItemImages
            //                                    on d.ItemId equals img.ItemId
            //                                    where d.SectionId == subSectId
            //                                    select new UhsbItemImageVM
            //                                    {
            //                                        ImageId = img.ImageId,
            //                                        ItemId = d.ItemId,
            //                                        ImageUrl = relativePath + (img.ImageUrl ?? string.Empty).Replace("\\", "/"),
            //                                        Description = img.Description,
            //                                        ItemName = _context.UhsbItemDeails.Where(c=>c.ItemId == d.ItemId).Select(c=>c.Name).FirstOrDefault()
            //                                    };


            IQueryable<UhsbItemImageVM> query = from map in _context.UHSB_SectionsMappings
                        join section in _context.UhsbSections
                            on map.SectionId equals section.SectionId
                        join item in _context.UhsbItemDeails
                            on map.SectionMapId equals item.SectionMapId
                        join img in _context.UhsbItemImages
                            on item.ItemId equals img.ItemId
                        where map.CropId == cropid && section.SectionId == subSectId
                                                select new UhsbItemImageVM
                        {
                            ImageId = img.ImageId,
                            ItemId = item.ItemId,
                            ImageUrl = relativePath + (img.ImageUrl ?? string.Empty).Replace("\\", "/"),
                            Description = img.Description,
                            ItemName = item.Name
                        };

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
            var totalCount = await query.CountAsync();

            // Ordering (example, can extend for more fields)
            //query = orderBy switch
            //{
            //    GridEnum.FTPDocumentsLogs.BranchName => isDescending ? query.OrderByDescending(x => x.ItemId) : query.OrderBy(x => x.ItemId),
            //    _ => query
            //};

            // Paging
            var dataList = await query.Skip((currentPage - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();
             
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
        public async Task<GenericGridModel<UserMasterVM>> GetGridUsermasterV2(int currentPage = 1, int pageSize = 10, GridEnum.FTPDocumentsLogs orderBy = GridEnum.FTPDocumentsLogs.BranchName, bool isDescending = false, string filterDetails = null, string externalFilter = null)
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
                                      .Take(pageSize).OrderByDescending(x => x.UserName)
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
                var cropItem = new UhsbItemImage
                { 
                    ItemId = item.ItemId,
                    Description = item.Description,
                    ImageUrl = item.ImageUrl
                };

                _context.UhsbItemImages.Add(cropItem);
            }
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion
    }
}
