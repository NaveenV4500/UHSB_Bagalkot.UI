using AutoMapper;
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
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class CetegoryRepository:CommonConnection, ICetegoryRepository
    {
        private readonly IMapper _mapper;

        public CetegoryRepository(Uhsb2025uatContext context) : base(context)
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
        #region Category 
        public async Task<GenericGridModel<CategoryVM>> GetGridCategoryV2(int currentPage = 1, int pageSize = 10, GridEnum.FTPDocumentsLogs orderBy = GridEnum.FTPDocumentsLogs.BranchName, bool isDescending = false, string filterDetails = null, string externalFilter = null)
        {
            var relativePath = "";

            IQueryable<CategoryVM> query = from um in _context.UhsbCategories
                                             select new CategoryVM
                                             {
                                                 CategoryId = um.CategoryId,
                                                 Name = um.Name,
                                                 ImageUrl = um.ImageUrl 
                                             };

            List<GridFilterModel> filters = null;
            if (!string.IsNullOrEmpty(filterDetails))
            {
                filters = JsonConvert.DeserializeObject<List<GridFilterModel>>(filterDetails);
                if (filters != null && filters.Count > 0)
                {
                    foreach (var filter in filters)
                    {
                        
                    }
                }
            }

            var totalCount = await query.CountAsync();


            var dataList = await query.Skip((currentPage - 1) * pageSize)
                                      .Take(pageSize).OrderByDescending(x => x.CategoryId)
                                      .ToListAsync();

            var data = new GenericGridModel<CategoryVM>
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
        //public async Task<IEnumerable<CategoryVM>> GetAllAsync()
        //{
        //    return await _context.UhsbCategories
        //        .Select(c => new CategoryVM
        //        {
        //            CategoryId = c.CategoryId,
        //            Name = c.Name,
        //            ImageUrl = c.ImageUrl
        //        })
        //        .ToListAsync();
        //}

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
