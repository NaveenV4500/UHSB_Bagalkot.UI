using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class CetegoryRepository:CommonConnection, ICetegoryRepository
    {
        private readonly IMapper _mapper;

        public CetegoryRepository(Uhsb2025Context context) : base(context)
        {

        }
        public async Task<CategoryVM> AddAsync(CategoryVM categoryVM)
        {
            var entity = new UhsbCategory
            {
                Name = categoryVM.Name,
                ImageUrl = categoryVM.ImageUrl
            };

            _context.UhsbCategories.Add(entity);
            await _context.SaveChangesAsync();

            // Map back
            categoryVM.CategoryId = entity.CategoryId;
            return categoryVM;
        }

        public async Task<CategoryVM?> UpdateAsync(CategoryVM categoryVM)
        {
            var existing = await _context.UhsbCategories.FindAsync(categoryVM.CategoryId);
            if (existing == null) return null;

            existing.Name = categoryVM.Name;
            existing.ImageUrl = categoryVM.ImageUrl;

            await _context.SaveChangesAsync();

            return new CategoryVM
            {
                CategoryId = existing.CategoryId,
                Name = existing.Name,
                ImageUrl = existing.ImageUrl
            };
        }

        public async Task<IEnumerable<CategoryVM>> GetAllAsync()
        {
            return await _context.UhsbCategories
                .Select(c => new CategoryVM
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    ImageUrl = c.ImageUrl
                })
                .ToListAsync();
        }

        public async Task<CategoryVM?> GetByIdAsync(int id)
        {
            var entity = await _context.UhsbCategories.FindAsync(id);
            if (entity == null) return null;

            return new CategoryVM
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                ImageUrl = entity.ImageUrl
            };
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.UhsbCategories.FindAsync(id);
            if (existing == null) return false;

            _context.UhsbCategories.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
