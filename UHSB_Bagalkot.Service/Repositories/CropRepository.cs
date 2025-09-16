using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels.Crop;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class CropRepository:CommonConnection,ICropRepository
    {
        private readonly IMapper _mapper;

        public CropRepository(Uhsb2025Context context) : base(context)
        {

        }
        public async Task<CropDetailsVM> AddAsync(CropDetailsVM CropDetailsVM)
        {
            var crop = new UhsbCrop
            {
                CategoryId = CropDetailsVM.CategoryId,
                Name = CropDetailsVM.Name,
                Description = CropDetailsVM.Description,
                ImageUrl = CropDetailsVM.ImageUrl
            };

            _context.UhsbCrops.Add(crop);
            await _context.SaveChangesAsync();

            CropDetailsVM.CropId = crop.CropId;
            return CropDetailsVM;
        }

        public async Task<CropDetailsVM?> UpdateAsync(CropDetailsVM CropDetailsVM)
        {
            var existing = await _context.UhsbCrops.FindAsync(CropDetailsVM.CropId);
            if (existing == null) return null;

            existing.CategoryId = CropDetailsVM.CategoryId;
            existing.Name = CropDetailsVM.Name;
            existing.Description = CropDetailsVM.Description;
            existing.ImageUrl = CropDetailsVM.ImageUrl;

            await _context.SaveChangesAsync();
            return CropDetailsVM;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var crop = await _context.UhsbCrops.FindAsync(id);
            if (crop == null) return false;

            _context.UhsbCrops.Remove(crop);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CropDetailsVM?> GetByIdAsync(int id)
        {
            var crop = await _context.UhsbCrops.FindAsync(id);
            if (crop == null) return null;

            return new CropDetailsVM
            {
                CropId = crop.CropId,
                CategoryId = crop.CategoryId,
                Name = crop.Name,
                Description = crop.Description,
                ImageUrl = crop.ImageUrl
            };
        }

        public async Task<IEnumerable<CropDetailsVM>> GetAllAsync()
        {
            return await _context.UhsbCrops
                .Select(c => new CropDetailsVM
                {
                    CropId = c.CropId,
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl
                })
                .ToListAsync();
        }
    }
}
