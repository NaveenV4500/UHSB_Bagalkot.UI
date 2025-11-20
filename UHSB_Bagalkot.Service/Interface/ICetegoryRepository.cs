using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface ICetegoryRepository
    {
        //Save and Update
        Task<CategoryVM> AddAsync(CategoryVM category);
        Task<CategoryVM?> UpdateAsync(CategoryVM category);
        //Task<IEnumerable<CategoryVM>> GetAllAsync();
        Task<GenericGridModel<CategoryVM>> GetGridCategoryV2(int currentPage = 1, int pageSize = 10, GridEnum.FTPDocumentsLogs orderBy = GridEnum.FTPDocumentsLogs.BranchName, bool isDescending = false, string filterDetails = null, string externalFilter = null);

        Task<CategoryVM?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
