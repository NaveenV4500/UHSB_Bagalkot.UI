using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Data.DataAccess;
using UHSB_Bagalkot.Service.Interface;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class HorticultureHandbookRepository:CommonConnection, IHorticultureHandbookRepository
    {
        public HorticultureHandbookRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CropCategory>> GetHorticultureHandbookAsync()
        {
            return _context.CropCategories.ToList();
        }
        public async Task<IEnumerable<Crop>> GetHorticultureHandbookItemsAsync(int categoryId)
        {
            return await _context.Crops
                .Where(x => x.CategoryId == categoryId)
                .ToListAsync();
        }

    }
}
