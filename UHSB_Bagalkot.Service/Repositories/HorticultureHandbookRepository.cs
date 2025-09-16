using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Dto;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels.Crop;
using UHSB_Bagalkot.Service.ViewModels.Sections;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class HorticultureHandbookRepository:CommonConnection, IHorticultureHandbookRepository
    {
        public HorticultureHandbookRepository(Uhsb2025Context context) : base(context)
        {
        }

        public async Task<IEnumerable<UhsbCategory>> GetHorticultureHandbookAsync()
        {
            return _context.UhsbCategories.ToList();
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
                .Include(s => s.Crop)
                .Select(s => new UhsbSectionVM
                {
                    SectionId = s.SectionId,
                    CropId = s.CropId,
                    CropName = s.Crop.Name,
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

    }
}
