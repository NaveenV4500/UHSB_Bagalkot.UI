using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Service.ViewModels.Crop;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface ICropRepository
    {
        Task<CropDetailsVM> AddAsync(CropDetailsVM CropDetailsVM);
        Task<CropDetailsVM?> UpdateAsync(CropDetailsVM CropDetailsVM);
        Task<bool> DeleteAsync(int id);
        Task<CropDetailsVM?> GetByIdAsync(int id);
        Task<IEnumerable<CropDetailsVM>> GetAllAsync();
    }
}
