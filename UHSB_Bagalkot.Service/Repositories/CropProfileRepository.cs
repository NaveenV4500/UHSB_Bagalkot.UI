using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UHSB_Bagalkot.Service.Dto;
using UHSB_Bagalkot.Service.Interface;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class CropProfileRepository//:CommonConnection,ICropProfileRepository
    {
        //public CropProfileRepository(Uhsb2025Context context) : base(context)
        //{
        //}

        //public async Task<CropFullDetailsDto> GetCropFullDetailsAsync(int cropId)
        //{
        //    var crop = await _context.Crops
        //        .FirstOrDefaultAsync(c => c.CropId == cropId);

        //    if (crop == null) return null;

        //    var cropDetails = await _context.CropDetails
        //        .Where(cd => cd.CropId == cropId)
        //        .ToListAsync();

        //    var articles = await _context.Articles
        //        .Include(a => a.ArticleItems)
        //        .Where(a => a.CropId == cropId)
        //        .ToListAsync();

        //    return new CropFullDetailsDto
        //    {
        //        Crop = crop,
        //        CropDetails = cropDetails,
        //        Articles = articles
        //    };
        //}
    }
}
