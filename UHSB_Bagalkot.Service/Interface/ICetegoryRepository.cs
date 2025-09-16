using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface ICetegoryRepository
    {
        //Save and Update
        Task<CategoryVM> AddAsync(CategoryVM category);
        Task<CategoryVM?> UpdateAsync(CategoryVM category);
        Task<IEnumerable<CategoryVM>> GetAllAsync();
        Task<CategoryVM?> GetByIdAsync(int id);
    }
}
