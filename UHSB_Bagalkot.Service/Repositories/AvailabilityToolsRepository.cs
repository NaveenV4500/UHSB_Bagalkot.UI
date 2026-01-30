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
using UHSB_Bagalkot.Service.ViewModels;
using UHSB_Bagalkot.Service.ViewModels.AvailabilityTools;
using UHSB_Bagalkot.Service.ViewModels.Crop;
using UHSB_Bagalkot.Service.ViewModels.Product;
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

        #region Get varieties 

        public async Task<List<ProductVariety_SP_VM>> GetProductVarieties(int productid=0)
        {
            try
            {
 
                var queryPrtd = new List<ProductVariety_SP_VM>();

                //var queryPrtd = _context.UhsbProducts.AsQueryable();

                #region get data using SP
                string connectionString = GetConnectionString();

                using SqlConnection con = new(connectionString);
                using SqlCommand cmd = new("USP_GetProductVarieties", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@ProductId", productid);


                await con.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    queryPrtd.Add(new ProductVariety_SP_VM
                    {
                        VarietiesId = reader["VarietiesId"] != DBNull.Value
                            ? Convert.ToInt32(reader["VarietiesId"])
                            : 0,

                        ProductId = reader["ProductId"] != DBNull.Value
                            ? Convert.ToInt32(reader["ProductId"])
                            : 0,

                        ProductName_eng = reader["ProductName_eng"]?.ToString() ?? string.Empty,
                        ProductName_knd = reader["ProductName_knd"]?.ToString() ?? string.Empty,

                        CenterId = reader["CenterId"] != DBNull.Value
                            ? Convert.ToInt32(reader["CenterId"])
                            : 0,

                        Centername_eng = reader["Centername_eng"]?.ToString() ?? string.Empty,

                        DistrictName = reader["DistrictName"]?.ToString() ?? string.Empty,

                        VarietyName_eng = reader["VarietyName_eng"]?.ToString() ?? string.Empty,
                        VarietyName_knd = reader["VarietyName_knd"]?.ToString() ?? string.Empty,

                        Stock_Keeping_Unit = reader["Stock_Keeping_Unit"]?.ToString() ?? string.Empty,
                        Barcode = reader["Barcode"]?.ToString() ?? string.Empty,

                        UnitId = reader["UnitId"] != DBNull.Value
                            ? Convert.ToInt32(reader["UnitId"])
                            : (int?)null,

                        UnitName_eng = reader["UnitName_eng"]?.ToString() ?? string.Empty,

                        Quantity = reader["Quantity"] != DBNull.Value
                            ? Convert.ToDecimal(reader["Quantity"])
                            : (decimal?)null,

                        MRPPrice = reader["MRPPrice"] != DBNull.Value
                            ? Convert.ToDecimal(reader["MRPPrice"])
                            : 0,

                        SellingPrice = reader["SellingPrice"] != DBNull.Value
                            ? Convert.ToDecimal(reader["SellingPrice"])
                            : 0,

                        StockQty = reader["StockQty"] != DBNull.Value
                            ? Convert.ToInt32(reader["StockQty"])
                            : 0,

                        MinStockQty = reader["MinStockQty"] != DBNull.Value
                            ? Convert.ToInt32(reader["MinStockQty"])
                            : (int?)null,

                        IsActive = reader["IsActive"] != DBNull.Value
                            ? Convert.ToBoolean(reader["IsActive"])
                            : false,

                        filepath = reader["filepath"]?.ToString() ?? string.Empty,
                        Remarks = reader["Remarks"]?.ToString() ?? string.Empty,

                        CreatedBy = reader["CreatedBy"] != DBNull.Value
                            ? Convert.ToInt32(reader["CreatedBy"])
                            : (int?)null,

                        CreatedDate = reader["CreatedDate"] != DBNull.Value
                            ? Convert.ToDateTime(reader["CreatedDate"])
                            : (DateTime?)null,

                        ModifiedBy = reader["ModifiedBy"] != DBNull.Value
                            ? Convert.ToInt32(reader["ModifiedBy"])
                            : (int?)null,

                        ModifiedDate = reader["ModifiedDate"] != DBNull.Value
                            ? Convert.ToDateTime(reader["ModifiedDate"])
                            : (DateTime?)null
                    });
                }


                CommonEnum.WriteLog($"Repository: Raw items fetched = {queryPrtd.Count}");
                #endregion

                //var vmList = _mapper.Map<List<Product_SP_VM>>(dataList);

                return queryPrtd;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        #endregion 


        #region Get Product

        public async Task<GenericGridModel<Product_SP_VM>> getgridcontentproducts(
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
            try
            {
                CommonEnum.WriteLog($"Repository: Fetching ProductsVM | DistrictId={districtid}, CenterId={centerid}, PageType={pagetype}");

                var queryPrtd = new List<Product_SP_VM>();

                //var queryPrtd = _context.UhsbProducts.AsQueryable();

                #region get data using SP
                string connectionString = GetConnectionString();

                using SqlConnection con = new(connectionString);
                using SqlCommand cmd = new("USP_GetProductWithVariets", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
               



                await con.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    queryPrtd.Add(new Product_SP_VM
                    {
                        ProductId = reader["ProductId"] != DBNull.Value
                     ? Convert.ToInt32(reader["ProductId"])
                     : 0,

                        DistrictName = reader["DistrictName"]?.ToString() ?? string.Empty,

                        Centername_eng = reader["Centername_eng"]?.ToString() ?? string.Empty,

                        RecordHead_eng = reader["RecordHead_eng"]?.ToString() ?? string.Empty,

                        ProductName_eng = reader["ProductName_eng"]?.ToString() ?? string.Empty,
                        ProductName_knd = reader["ProductName_knd"]?.ToString() ?? string.Empty,

                        Remarks = reader["Remarks"]?.ToString() ?? string.Empty,

                        Filepath = reader["filepath"]?.ToString() ?? string.Empty,

                        IsActive = reader["IsActive"] != DBNull.Value
                     ? Convert.ToBoolean(reader["IsActive"])
                     : false,

                        CreatedBy = reader["CreatedBy"] != DBNull.Value
                     ? Convert.ToInt32(reader["CreatedBy"])
                     : (int?)null,

                        CreatedDate = reader["CreatedDate"] != DBNull.Value
                     ? Convert.ToDateTime(reader["CreatedDate"])
                     : (DateTime?)null,

                        ModifiedBy = reader["ModifiedBy"] != DBNull.Value
                     ? Convert.ToInt32(reader["ModifiedBy"])
                     : (int?)null,

                        ModifiedDate = reader["ModifiedDate"] != DBNull.Value
                     ? Convert.ToDateTime(reader["ModifiedDate"])
                     : (DateTime?)null
                    });

                }

                CommonEnum.WriteLog($"Repository: Raw items fetched = {queryPrtd.Count}");
                #endregion

                var query = queryPrtd.AsQueryable();

                if (!string.IsNullOrEmpty(filterDetails))
                {
                    var filters = JsonConvert.DeserializeObject<List<GridFilterModel>>(filterDetails);
                    if (filters != null && filters.Count > 0)
                    {
                        foreach (var filter in filters)
                        {
                            var predicate = GetWherePredictionForProduct(
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

                var dataList = queryPrtd.Skip((currentPage - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
                //var vmList = _mapper.Map<List<Product_SP_VM>>(dataList);

                return new GenericGridModel<Product_SP_VM>
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
            catch (Exception ex)
            {
                return new GenericGridModel<Product_SP_VM>
                {
                    ItemDetails = null,
                    TotalCount = 0,
                    currentPage = currentPage,
                    CanAdd = true,
                    CanEdit = true,
                    CanDelete = true,
                    CanViewSingle = true,
                    CanViewMultiple = true
                };
            }

        }


        // Filtering logic
        public Expression<Func<Product_SP_VM, bool>> GetWherePredictionForProduct(GridEnum.AvailabilityToolsFilterBy filterBy, string filterTxt, GridEnum.FilterTypeEnum filterType)
        {
            Expression<Func<Product_SP_VM, bool>> predicate = null;

            if (!string.IsNullOrEmpty(filterTxt))
            {
                switch (filterBy)
                {
                    case GridEnum.AvailabilityToolsFilterBy.AvailToolNameEng:
                        switch (filterType)
                        {
                            case GridEnum.FilterTypeEnum.Equal:
                                predicate = x => x.ProductName_eng.Equals(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.Contains:
                                predicate = x => x.ProductName_eng.Contains(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.StartsWith:
                                predicate = x => x.ProductName_eng.StartsWith(filterTxt);
                                break;
                            case GridEnum.FilterTypeEnum.EndsWith:
                                predicate = x => x.ProductName_eng.EndsWith(filterTxt);
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

        #endregion

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
                    Name = c.CenternameEng + " - " + c.CenternameKnd,
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



        #region Product SaveOrEditProdectDetails

        public async Task<bool> SaveOrEditProdectDetailsOld(ProductsVM obj)
        {
            try
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(ProductsVM));

                if (obj.ProductId == 0)
                {
                    var productEntity = _mapper.Map<UhsbProduct>(obj);
                    productEntity.CreatedBy = 1;
                    productEntity.ModifiedBy = 1;
                    productEntity.CreatedDate = DateTime.Now;
                    productEntity.ModifiedDate = DateTime.Now;
                    _context.UhsbProducts.Add(productEntity);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    var existing = await _context.UhsbProducts.FindAsync(obj.ProductId);

                    if (existing == null)
                        return false;

                    if (!string.IsNullOrEmpty(obj.Filepath))
                    {
                        existing.Filepath = obj.Filepath;
                    }
                    existing.ModifiedBy = 1;
                    existing.ModifiedDate = DateTime.Now;
                    _mapper.Map(obj, existing);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<saveresponse> SaveOrEditProdectDetails(ProductsVM obj)
        {


            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            saveresponse res = new saveresponse();
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                UhsbProduct productEntity;
                

                if (obj.ProductId == 0)
                {
                    var exsting = _context.UhsbProducts.Where(x => x.ProductNameEng == obj.ProductNameEng || x.ProductNameKnd == obj.ProductNameKnd).FirstOrDefault();

                    if (exsting != null)
                    {
                        res.message = "The Product name is already in exsting .";
                        res.success = false;
                        return res;
                    }

                    // New Product
                    productEntity = _mapper.Map<UhsbProduct>(obj);
                    productEntity.CreatedBy = 1;
                    productEntity.ModifiedBy = 1;
                    productEntity.CreatedDate = DateTime.Now;
                    productEntity.ModifiedDate = DateTime.Now;

                    _context.UhsbProducts.Add(productEntity);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Existing Product
                    productEntity = await _context.UhsbProducts.FindAsync(obj.ProductId);


                    if (!string.IsNullOrEmpty(obj.Filepath))
                        productEntity.Filepath = obj.Filepath;

                    obj.ModifiedBy = 1;
                    obj.ModifiedDate = DateTime.Now;
                    obj.CreatedDate = productEntity.CreatedDate;
                    if (obj.Filepath == null || obj.Filepath == "")
                    {
                        obj.Filepath = productEntity.Filepath;

                    }
                    _mapper.Map(obj, productEntity);
                    await _context.SaveChangesAsync();
                }

                // ---- Save Varieties ----
                if (obj.ProductVarietyItems != null && obj.ProductVarietyItems.Any())
                {
                    foreach (var variety in obj.ProductVarietyItems)
                    {

                        if (variety.VarietiesId == 0)
                        {
                            var exstingVar = _context.UhsbProductVarieties.Where(x => x.VarietyNameEng == variety.VarietyNameEng || x.VarietyNameKnd == variety.VarietyNameKnd).FirstOrDefault();

                            if (exstingVar != null)
                            {
                                res.message = "The Variety name is already in exsting .";
                                res.success = false;
                                return res;
                            }
                            // New variety
                            var varietyEntity = _mapper.Map<UhsbProductVariety>(variety);
                            varietyEntity.ProductId = productEntity.ProductId;
                            varietyEntity.CenterId = productEntity.CenterId; 
                            varietyEntity.CreatedBy = 1;
                            varietyEntity.ModifiedBy = 1;
                            varietyEntity.CreatedDate = DateTime.Now;
                            varietyEntity.ModifiedDate = DateTime.Now;
                            _context.UhsbProductVarieties.Add(varietyEntity);
                        }
                        else
                        {
                            // Existing variety
                            var existingVariety = await _context.UhsbProductVarieties
                                   .FirstOrDefaultAsync(x => x.VarietiesId == variety.VarietiesId);

                            if (existingVariety != null)
                            {
                                // ===== ONLY UPDATABLE FIELDS =====
                                existingVariety.VarietyNameEng = variety.VarietyNameEng;
                                existingVariety.VarietyNameKnd = variety.VarietyNameKnd;
                                existingVariety.UnitId = variety.UnitId;
                                existingVariety.Mrpprice = variety.Mrpprice;
                                existingVariety.SellingPrice = variety.SellingPrice;
                                existingVariety.StockQty = variety.StockQty;
                                existingVariety.MinStockQty = variety.MinStockQty; 
                                existingVariety.Remarks = variety.Remarks;

                                // ===== IMAGE HANDLING =====
                                if (!string.IsNullOrWhiteSpace(variety.Filepath))
                                {
                                    existingVariety.Filepath = variety.Filepath;
                                }
                                // else → keep old filepath

                                // ===== AUDIT =====
                                existingVariety.ModifiedBy = 1;
                                existingVariety.ModifiedDate = DateTime.Now;
                                productEntity.CreatedDate = productEntity.CreatedDate;
                            }

                        }
                    }

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync(); // Commit both product & varieties together
                return new saveresponse
                {
                    message = "",
                    success = true
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback if any error
                 
                    res.message = "Internal Server Error.";
                    res.success = false;
                    return res;
              
            }
        }


        public async Task<ProductsVM?> GetbyIdProdect(int identifier)
        {
            if (identifier <= 0)
                return null;

            try
            {
                // 1️⃣ Get Product
                var product = await (
                    from prd in _context.UhsbProducts.AsNoTracking()
                    join scm in _context.UhsbSeedPlantingCenterMasters.AsNoTracking()
                        on prd.CenterId equals scm.CenterId
                    join rhm in _context.UhsbRecordHeadMasters.AsNoTracking()
                        on prd.HeadId equals rhm.HeadId
                    where prd.ProductId == identifier
                    select new ProductsVM
                    {
                        ProductId = prd.ProductId,
                        CenterId = prd.CenterId,
                        DistrictId = scm.DistrictId ?? 0,
                        HeadId = prd.HeadId,

                        ProductNameEng = prd.ProductNameEng,
                        ProductNameKnd = prd.ProductNameKnd,

                        Remarks = prd.Remarks,
                        CreatedBy = prd.CreatedBy,
                        CreatedDate = prd.CreatedDate,
                        ModifiedBy = prd.ModifiedBy,
                        ModifiedDate = prd.ModifiedDate,
                        IsActive = prd.IsActive
                    }
                ).FirstOrDefaultAsync();

                if (product == null)
                    return null;

                // 2️⃣ Get Variety Items
                product.ProductVarietyItems = await (
                    from v in _context.UhsbProductVarieties.AsNoTracking()
                    where v.ProductId == identifier 
                    select new ProductVarietyVM
                    {
                        VarietiesId = v.VarietiesId,
                        ProductId = v.ProductId,
                        CenterId = v.CenterId,

                        VarietyNameEng = v.VarietyNameEng,
                        VarietyNameKnd = v.VarietyNameKnd,

                        UnitId = v.UnitId,
                        Mrpprice = v.Mrpprice,
                        SellingPrice = v.SellingPrice,
                        StockQty = v.StockQty,
                        MinStockQty = v.MinStockQty,

                        StockKeepingUnit = v.StockKeepingUnit,
                        Barcode = v.Barcode,
                        Quantity = v.Quantity,

                        IsActive = v.IsActive,
                        Filepath = v.Filepath,
                        Remarks = v.Remarks,

                        CreatedBy = v.CreatedBy,
                        CreatedDate = v.CreatedDate,
                        ModifiedBy = v.ModifiedBy,
                        ModifiedDate = v.ModifiedDate
                    }
                ).ToListAsync();

                return product;
            }
            catch (Exception ex)
            {
                // log ex
                return null;
            }
        }



        #endregion
    }
}
