using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface IHorticultureHandbookRepository
    {
        Task<IEnumerable<CropCategory>> GetHorticultureHandbookAsync();  
        Task<IEnumerable<Crop>> GetHorticultureHandbookItemsAsync(int categoryId); 
    }
}
