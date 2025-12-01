using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
using UHSB_Bagalkot.Service.ViewModels.AvailabilityTools;
using UHSB_Bagalkot.Service.ViewModels.Sections;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class AvailabilityToolsRepository : CommonConnection, IAvailabilityToolsRepository
    {
        private readonly IMapper _mapper; 

        public AvailabilityToolsRepository(Uhsb2025uatContext context, IMapper mapper)
            : base(context)
        {
            _mapper = mapper;
        }
         
        public string GetConnectionString()
        {
            return _context.Database.GetDbConnection().ConnectionString;
        }
        public async Task<List<UhsbSeedPlantingCenterMasterVM>> GetRecordHeadMaster(int districtId = 0)
        {
            var entity = await _context.UhsbSeedPlantingCenterMasters
                                       .Where(x => x.DistrictId == districtId)
                                       .ToListAsync();

            if (entity == null || entity.Count == 0)
                return new List<UhsbSeedPlantingCenterMasterVM>();

            return _mapper.Map<List<UhsbSeedPlantingCenterMasterVM>>(entity);
        }
        public async Task<List<UhsbSeedPlantingCenterMasterVM>> GetCenterByDistrict(int districtId = 0)
        {
            var entity = await _context.UhsbSeedPlantingCenterMasters
                                       .Where(x => x.DistrictId == districtId)
                                       .ToListAsync();

            if (entity == null || entity.Count == 0)
                return new List<UhsbSeedPlantingCenterMasterVM>();

            return _mapper.Map<List<UhsbSeedPlantingCenterMasterVM>>(entity);
        }
        //getgridcontentavailabilitytools
        public async Task<GenericGridModel<AvailabilityToolsDetailsVM>> getgridcontentavailabilitytools(
            int currentPage = 1,
            int pageSize = 10,
            GridEnum.AvailabilityToolsFilterBy orderBy = GridEnum.AvailabilityToolsFilterBy.CreatedDate,
            bool isDescending = false,
            string filterDetails = null,
            string externalFilter = null,
            int centerid = 0,
            int districtid = 0,
            int pagetype = 0)
        {
            CommonEnum.WriteLog($"Repository: Fetching AvailabilityTools | DistrictId={districtid}, CenterId={centerid}, PageType={pagetype}");

            var items = new List<AvailabilityToolsDetailsVM>();
            string connectionString = GetConnectionString();

            using SqlConnection con = new(connectionString);
            using SqlCommand cmd = new("USP_GetAvailabilityToolsDetails", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DistrictId", districtid);
            cmd.Parameters.AddWithValue("@CenterId", centerid);
            cmd.Parameters.AddWithValue("@pagetype", pagetype);

            await con.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new AvailabilityToolsDetailsVM
                {
                    Identifier = reader["identifier"] != DBNull.Value ? Convert.ToInt32(reader["identifier"]) : 0,
                    CenterId = reader["CenterId"] != DBNull.Value ? Convert.ToInt32(reader["CenterId"]) : 0,
                    Centername_eng = reader["Centername_eng"]?.ToString() ?? string.Empty,
                    Centername_knd = reader["Centername_knd"]?.ToString() ?? string.Empty,
                    DistrictId = reader["DistrictId"] != DBNull.Value ? Convert.ToInt32(reader["DistrictId"]) : 0,
                    HeadId = reader["HeadId"] != DBNull.Value ? Convert.ToInt32(reader["HeadId"]) : 0,
                    RecordHead_eng = reader["RecordHead_eng"]?.ToString() ?? string.Empty,
                    RecordHead_knd = reader["RecordHead_knd"]?.ToString() ?? string.Empty,
                    AvailToolname_eng = reader["AvailToolname_eng"]?.ToString() ?? string.Empty,
                    AvailToolname_knd = reader["AvailToolname_knd"]?.ToString() ?? string.Empty,
                    Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : (int?)null,
                    Unit = reader["Unit"]?.ToString() ?? string.Empty,
                    Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : (decimal?)null,
                    AvailabilityDate = reader["AvailabilityDate"] != DBNull.Value ? Convert.ToDateTime(reader["AvailabilityDate"]) : (DateTime?)null,
                    Remarks = reader["Remarks"]?.ToString() ?? string.Empty,
                    CreatedBy = reader["CreatedBy"] != DBNull.Value ? Convert.ToInt32(reader["CreatedBy"]) : (int?)null,
                    CreatedDate = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : (DateTime?)null,
                    ModifiedBy = reader["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(reader["ModifiedBy"]) : (int?)null,
                    ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedDate"]) : (DateTime?)null
                });
            }

            CommonEnum.WriteLog($"Repository: Raw items fetched = {items.Count}");

            var query = items.AsQueryable();

            if (!string.IsNullOrEmpty(filterDetails))
            {
                var filters = JsonConvert.DeserializeObject<List<GridFilterModel>>(filterDetails);
                if (filters != null && filters.Count > 0)
                {
                    foreach (var filter in filters)
                    {
                        var predicate = GetWherePrediction(
                            (GridEnum.AvailabilityToolsFilterBy)filter.filterBy,
                            (filter.filterTxt ?? "").Trim(),
                            (GridEnum.FilterTypeEnum)filter.filterType
                        );

                        if (predicate != null)
                            query = query.Where(predicate);
                    }
                }
            }

            var totalCount = query.Count();

            CommonEnum.WriteLog($"Repository: After filtering rows = {totalCount}");

            var dataList = query.Skip((currentPage - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            return new GenericGridModel<AvailabilityToolsDetailsVM>
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
        }

        // Filtering logic
        public Expression<Func<AvailabilityToolsDetailsVM, bool>> GetWherePrediction(GridEnum.AvailabilityToolsFilterBy filterBy, string filterTxt, GridEnum.FilterTypeEnum filterType)
        {
            Expression<Func<AvailabilityToolsDetailsVM, bool>> predicate = null;

            if (!string.IsNullOrEmpty(filterTxt))
            {
                switch (filterBy)
                {
                    case GridEnum.AvailabilityToolsFilterBy.AvailToolNameKnd:
                        switch (filterType)
                        {
                            case GridEnum.FilterTypeEnum.Equal:
                                predicate = x => x.Centername_eng.Equals(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.Contains:
                                predicate = x => x.Centername_eng.Contains(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.StartsWith:
                                predicate = x => x.Centername_eng.StartsWith(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.EndsWith:
                                predicate = x => x.Centername_eng.EndsWith(filterTxt);
                                break;
                            default:
                                break;
                        }
                        break;
                    case GridEnum.AvailabilityToolsFilterBy.RecordHeadKnd:
                        switch (filterType)
                        {
                            case GridEnum.FilterTypeEnum.Equal:
                                predicate = x => x.RecordHead_eng.Equals(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.Contains:
                                predicate = x => x.Centername_eng.Contains(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.StartsWith:
                                predicate = x => x.Centername_eng.StartsWith(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.EndsWith:
                                predicate = x => x.Centername_eng.EndsWith(filterTxt);
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

        /// <summary>
        /// plantingcentersDD
        /// </summary>
        /// <returns></returns>
        public async Task<List<DropdownItemDto>> plantingcentersDD()
        {
            return await _context.UhsbSeedPlantingCenterMasters
                .OrderBy(c => c.CenternameEng)
                .Select(c => new DropdownItemDto
                {
                    Id = c.CenterId,
                    Name = c.CenternameEng +" - "+ c.CenternameKnd,
                })
                .ToListAsync();
        }

       
        public async Task<List<DropdownItemDto>> RecordHeadTypeDD()
        {
            return await _context.UhsbRecordHeadMasters
                .OrderBy(c => c.RecordHeadEng)
                .Select(c => new DropdownItemDto
                {
                    Id = c.HeadId,
                    Name = c.RecordHeadEng + " - " + c.RecordHeadKnd,
                })
                .ToListAsync();
        }
    }
}
