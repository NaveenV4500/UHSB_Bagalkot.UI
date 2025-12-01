using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.Dto;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels.Crop;
using UHSB_Bagalkot.Service.ViewModels.Sections;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class HorticultureHandbookRepository:CommonConnection, IHorticultureHandbookRepository
    {
        private readonly IMapper _mapper;

        public HorticultureHandbookRepository(Uhsb2025uatContext context, IMapper mapper)
            : base(context)
        {
            _mapper = mapper;
        }

        public async Task<List<CategoryGridVM>> GetGridContentCategories()
        {
            var categories = await _context.UhsbCategories
                .AsNoTracking()
                .ToListAsync();

            var categoryIds = categories.Select(c => c.CategoryId).ToList();

            var imageFiles = await _context.UhsbImageFiles
                .AsNoTracking()
                .Where(f => categoryIds.Contains(f.ItemId??0))
                .ToListAsync();

            var mapdata = _mapper.Map<List<CategoryGridVM>>(categories);
            var mappedFiles = _mapper.Map<List<UhsbImageFileGridVM>>(imageFiles);

            var fileLookup = mappedFiles
                .GroupBy(f => f.ItemId?? 0)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var item in mapdata)
            {


                if (fileLookup.TryGetValue(item.CategoryId, out var files))
                    item.Files = files;
                else
                    item.Files = new List<UhsbImageFileGridVM>();
            }

            return mapdata;
        }

        public async Task<List<CropGridVM>> GetgridContentCrop()
        {
            var crops = await _context.UhsbCrops.AsNoTracking().ToListAsync();
            var cropsIds = crops.Select(c => c.CategoryId).ToList();
            var imageFiles = await _context.UhsbImageFiles.AsNoTracking().Where(f => cropsIds.Contains(f.ItemId ?? 0)).ToListAsync();
            var mapdata = _mapper.Map<List<CropGridVM>>(crops);
            var mappedFiles = _mapper.Map<List<UhsbImageFileGridVM>>(imageFiles);

            var fileLookup = mappedFiles
                .GroupBy(f => f.ItemId ?? 0)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var item in mapdata)
            {

                item.CategoryName = _context.UhsbCategories.Where(x => x.CategoryId == item.CategoryId).Select(x => x.Name).SingleOrDefault();

                if (fileLookup.TryGetValue(item.CropId, out var files))
                    item.Files = files;
                else
                    item.Files = new List<UhsbImageFileGridVM>();
            }
         
            return mapdata;
        }

        public async Task<bool> SaveOrEditCrops(CropDetailsVM cropVM)
        {
            if (cropVM.CropId == 0)
            {
                // Get the max CropId from DB and increment
                int maxId = 0;
                if (await _context.UhsbCrops.AnyAsync())
                    maxId = await _context.UhsbCrops.MaxAsync(c => c.CropId);

                cropVM.CropId = maxId + 1;

                var cropEntity = _mapper.Map<UhsbCrop>(cropVM);
                await _context.UhsbCrops.AddAsync(cropEntity);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                var existing = await _context.UhsbCrops.FindAsync(cropVM.CropId);
                if (existing == null) return false;
                if (cropVM.ImageUrl == null || cropVM.ImageUrl == "")
                {
                    cropVM.ImageUrl = existing.ImageUrl;
                }
                _mapper.Map(cropVM, existing);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<UhsbSectionVM>> GetgridContentSection()
        {
            var sections = await _context.UhsbSections.ToListAsync();
            return _mapper.Map<List<UhsbSectionVM>>(sections);
        }

        public async Task<List<ItemDetailsVM>> GetgridContentItemDetails(int categoryId = 0, int cropId = 0, int sectionId = 0)
        {
            var crops = await _context.UhsbItemDeails.ToListAsync();

            if (categoryId > 0)
            {
                crops = crops.Where(x => x.CategoryId == categoryId).ToList();
            }
            else if(cropId > 0)
            {
                crops = crops.Where(x => x.CropId == cropId).ToList();
            }
            else if (sectionId > 0)
            {
                crops = crops.Where(x => x.SectionId == sectionId).ToList();
            }
            var cropsIds = crops.Select(c => c.CategoryId).ToList();
            var imageFiles = await _context.UhsbImageFiles.AsNoTracking().Where(f => cropsIds.Contains(f.ItemId ?? 0)).ToListAsync();
            var mapdata = _mapper.Map<List<ItemDetailsVM>>(crops);
            var mappedFiles = _mapper.Map<List<UhsbImageFileGridVM>>(imageFiles);

            var fileLookup = mappedFiles
                .GroupBy(f => f.ItemId ?? 0)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var item in mapdata)
            {

                item.CategoryName = _context.UhsbCategories
                    .FirstOrDefault(x => x.CategoryId == item.CategoryId)?.Name;

                item.CropName = _context.UhsbCrops
                    .FirstOrDefault(x => x.CropId == item.CropId)?.Name;

                item.SectionName = _context.UhsbSections
                    .FirstOrDefault(x => x.SectionId == item.SectionId)?.Name;

                if (fileLookup.TryGetValue(item.CropId, out var files))
                    item.Files = files;
                else
                    item.Files = new List<UhsbImageFileGridVM>();
            }
 
            return mapdata;

        }

        public async Task<List<UhsbItemDeail>> GetgridContentItems()
        {
            return _context.UhsbItemDeails.ToList();
        }

        public async Task<IEnumerable<UhsbCrop>> GetHorticultureHandbookItemsAsync(int categoryId)
        {
            return await _context.UhsbCrops
                .Where(x => x.CategoryId == categoryId)
                .ToListAsync();
        }

        #region Section 
        public async Task<List<DropdownItemCropVM>> GetCropsForDD()
        {
            return await _context.UhsbCrops
                .Select(c => new DropdownItemCropVM
                {
                    Id = c.CropId,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<UhsbSectionVM>> GetAllSectionsAsync()
        {
            return await _context.UhsbSections
                .Select(s => new UhsbSectionVM
                {
                    SectionId = s.SectionId, 
                    Name = s.Name,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl
                })
                .ToListAsync();
        }

        public async Task<UhsbSectionVM?> GetSectionByIdAsync(int id)
        {
            return await _context.UhsbSections
                .Include(s => s.Crop)
                .Where(s => s.SectionId == id)
                .Select(s => new UhsbSectionVM
                {
                    SectionId = s.SectionId,
                    CropId = s.CropId,
                    CropName = s.Crop.Name,
                    Name = s.Name,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl
                })
                .FirstOrDefaultAsync();
        }

        public async Task<UhsbSectionVM> AddSectionAsync(UhsbSectionCreateUpdateVM model)
        {
            var entity = new UhsbSection
            {
                CropId = model.CropId,
                Name = model.Name,
                Description = model.Description,
                ImageUrl = model.ImageUrl
            };

            _context.UhsbSections.Add(entity);
            await _context.SaveChangesAsync();

            return new UhsbSectionVM
            {
                SectionId = entity.SectionId,
                CropId = entity.CropId,
                CropName = (await _context.UhsbCrops.FindAsync(entity.CropId))?.Name ?? "",
                Name = entity.Name,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl
            };
        }

        public async Task<UhsbSectionVM?> UpdateSectionAsync(int id, UhsbSectionCreateUpdateVM model)
        {
            var entity = await _context.UhsbSections.FindAsync(id);
            if (entity == null) return null;

            entity.CropId = model.CropId;
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.ImageUrl = model.ImageUrl;

            await _context.SaveChangesAsync();

            return new UhsbSectionVM
            {
                SectionId = entity.SectionId,
                CropId = entity.CropId,
                CropName = (await _context.UhsbCrops.FindAsync(entity.CropId))?.Name ?? "",
                Name = entity.Name,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl
            };
        }

        public async Task<bool> DeleteSectionAsync(int id)
        {
            var entity = await _context.UhsbSections.FindAsync(id);
            if (entity == null) return false;

            _context.UhsbSections.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }


        #endregion


        #region Items SaveOrEdit 


        public async Task<bool> SaveOrEditItemDetails(RequestItemDetailsVM ItemDetailsVM)
        {
            try
            {
                if (ItemDetailsVM == null)
                    throw new ArgumentNullException(nameof(ItemDetailsVM));

                if (ItemDetailsVM.ItemId == 0)
                {
                    int maxId = 0;
                    if (await _context.UhsbItemDeails.AnyAsync())
                        maxId = await _context.UhsbItemDeails.MaxAsync(c => c.ItemId);

                    ItemDetailsVM.ItemId = maxId + 1;

                    var cropEntity = _mapper.Map<UhsbItemDeail>(ItemDetailsVM);
                    
                    cropEntity.SubSectionId = 1;

                    cropEntity.SectionMapId = 1;

                    await _context.UhsbItemDeails.AddAsync(cropEntity);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    var existing = await _context.UhsbItemDeails.FindAsync(ItemDetailsVM.ItemId);
                    if (existing == null)
                        return false;

                    ItemDetailsVM.SectionMapId = 1;
                    if(ItemDetailsVM.ImageUrl == null || ItemDetailsVM.ImageUrl == "")
                    {
                        ItemDetailsVM.ImageUrl = existing.ImageUrl;

                    }
                    _mapper.Map(ItemDetailsVM, existing);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            { 
                 
                return false;
            }
        }


        public async Task<bool> DeleteItems(int id)
        {
            var entity = await _context.UhsbSections.FindAsync(id);
            if (entity == null) return false;

            _context.UhsbSections.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion


        #region Section SaveOrEditSectionDetails

        public async Task<bool> SaveOrEditSectionDetails(RequestSectionDetailsVM SectionDetailsVM)
        {
            try
            {
                if (SectionDetailsVM == null)
                    throw new ArgumentNullException(nameof(ItemDetailsVM));

                if (SectionDetailsVM.SectionId == 0)
                {
                    int maxId = 0;
                    if (await _context.UhsbSections.AnyAsync())
                        maxId = await _context.UhsbSections.MaxAsync(c => c.SectionId);

                    SectionDetailsVM.SectionId = maxId + 1;

                    var sectionEntity = _mapper.Map<UhsbSection>(SectionDetailsVM);
                    await _context.UhsbSections.AddAsync(sectionEntity);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    var existing = await _context.UhsbSections.FindAsync(SectionDetailsVM.SectionId);

                    if (existing == null)
                        return false;
                    if (SectionDetailsVM.ImageUrl == null || SectionDetailsVM.ImageUrl == "")
                    {
                        SectionDetailsVM.ImageUrl = existing.ImageUrl;
                    }
                    SectionDetailsVM.CropId = 1;
                    _mapper.Map(SectionDetailsVM, existing);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        #endregion


        #region Category SaveOrEditCategoryDetails

        public async Task<bool> SaveOrEditCategoryDetails(RequestCategoryDetailsVM categoryDetailsVM)
        {
            try
            {
                if (categoryDetailsVM == null)
                    throw new ArgumentNullException(nameof(CategoryGridVM));

                if (categoryDetailsVM.CategoryId == 0)
                {
                    int maxId = 0;
                    if (await _context.UhsbCategories.AnyAsync())
                        maxId = await _context.UhsbCategories.MaxAsync(c => c.CategoryId);

                    categoryDetailsVM.CategoryId = maxId + 1;

                    var categoryEntity = _mapper.Map<UhsbCategory>(categoryDetailsVM);
                    await _context.UhsbCategories.AddAsync(categoryEntity);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    var existing = await _context.UhsbCategories.FindAsync(categoryDetailsVM.CategoryId);

                    if (existing == null)
                        return false;
                    if (categoryDetailsVM.ImageUrl == null || categoryDetailsVM.ImageUrl == "")
                    {
                        categoryDetailsVM.ImageUrl = existing.ImageUrl;
                    }
                    _mapper.Map(categoryDetailsVM, existing);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        #endregion

        #region 

        public async Task<(bool Success, List<string> LinkedItems)> DeleteallpageItems(DeleteItemVM delmodel)
        {
            var linkedItems = new List<string>();

            if (delmodel == null)
            {
                linkedItems.Add("Invalid delete request.");
                return (false, linkedItems);
            }

            try
            {
                switch (delmodel.PageType)
                {
                    case CommonEnum.FileTypes.Category:
                        if (delmodel.CategoryId > 0)
                        {
                            var linkedCrops = await _context.UhsbCrops
                                .Where(x => x.CategoryId == delmodel.CategoryId)
                                .ToListAsync();

                            if (linkedCrops.Any())
                            {
                                linkedItems.Add($"Cannot delete this category. It is linked to {linkedCrops.Count} crop(s).");
                                return (false, linkedItems);
                            }

                            var delCategory = await _context.UhsbCategories.FindAsync(delmodel.CategoryId);
                            if (delCategory != null)
                            {
                                _context.UhsbCategories.Remove(delCategory);
                                await _context.SaveChangesAsync();
                                linkedItems.Add($"Category ID {delmodel.CategoryId} deleted successfully.");
                                return (true, linkedItems);
                            }
                        }
                        break;

                    case CommonEnum.FileTypes.Crops:
                        if (delmodel.CropId > 0)
                        {
                            var linkedItemsList = await _context.UhsbItemDeails
                                .Where(x => x.CropId == delmodel.CropId)
                                .ToListAsync();

                            if (linkedItemsList.Any())
                            {
                                linkedItems.Add($"Cannot delete this crop. It is linked to {linkedItemsList.Count} item(s).");
                                return (false, linkedItems);
                            }

                            var delCrop = await _context.UhsbCrops.FindAsync(delmodel.CropId);
                            if (delCrop != null)
                            {
                                _context.UhsbCrops.Remove(delCrop);
                                await _context.SaveChangesAsync();
                                linkedItems.Add($"Crop ID {delmodel.CropId} deleted successfully.");
                                return (true, linkedItems);
                            }
                        }
                        break;

                    case CommonEnum.FileTypes.Items:
                        if (delmodel.ItemDetailId > 0)
                        {
                            var linkedImages = await _context.UhsbItemImages
                                .Where(x => x.ItemId == delmodel.ItemDetailId)
                                .ToListAsync();

                            if (linkedImages.Any())
                            {
                                linkedItems.Add($"Cannot delete this item. It is linked to {linkedImages.Count} Content(s).");
                                return (false, linkedItems);
                            }

                            var delItem = await _context.UhsbItemDeails.FindAsync(delmodel.ItemDetailId);
                            if (delItem != null)
                            {
                                _context.UhsbItemDeails.Remove(delItem);
                                await _context.SaveChangesAsync();
                                linkedItems.Add($"Item ID {delmodel.ItemDetailId} deleted successfully.");
                                return (true, linkedItems);
                            }
                        }
                        break;

                    case CommonEnum.FileTypes.Sections:
                        if (delmodel.ItemDetailId > 0)
                        {
                            var delSection = await _context.UhsbSections.FindAsync(delmodel.ItemDetailId);
                            if (delSection != null)
                            {
                                _context.UhsbSections.Remove(delSection);
                                await _context.SaveChangesAsync();
                                linkedItems.Add($"Section ID {delmodel.ItemDetailId} deleted successfully.");
                                return (true, linkedItems);
                            }
                        }
                        break;

                    case CommonEnum.FileTypes.ItemContent:
                        if (delmodel.ItemDetailId > 0)
                        {
                            var delContent = await _context.UhsbItemImages.FindAsync(delmodel.ItemDetailId);
                            if (delContent != null)
                            {
                                _context.UhsbItemImages.Remove(delContent);
                                await _context.SaveChangesAsync();
                                linkedItems.Add($"Item content ID {delmodel.ItemDetailId} deleted successfully.");
                                return (true, linkedItems);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                linkedItems.Add("Error deleting item: " + ex.Message);
                return (false, linkedItems);
            }

            linkedItems.Add("No matching record found to delete.");
            return (false, linkedItems);
        }



        #endregion

    }
}
