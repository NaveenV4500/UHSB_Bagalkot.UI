using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Service.Common;

namespace UHSB_Bagalkot.Service.ViewModels.Sections
{
    public class UhsbSectionVM
    {
        public int SectionId { get; set; }

        public int CropId { get; set; }

        public string CropName { get; set; } = null!;
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string ImageUrl { get; set; } = null!;
        public  List<UhsbImageFileGridVM> Files { get; set; }
    }
    public class CategoryGridVM
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile ImageFile { get; set; }
        public string? filename { get; set; }

        public List<UhsbImageFileGridVM> Files { get; set; }

    }
    public class SectionsGridVM
    {
        public int SectionId { get; set; }

        public int CropId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? filename { get; set; }

        public List<UhsbImageFileGridVM>? Files { get; set; }
    }


    public class ItemDetailsVM
    {
        public int ItemId { get; set; }

        public int SubSectionId { get; set; }

        public string? Name { get; set; }

        public string? ImageUrl { get; set; }
        public string? filename { get; set; }

        public int? SectionMapId { get; set; }

        public int CropId { get; set; }

        public int CategoryId { get; set; }

        public int SectionId { get; set; }
        public string? CategoryName { get; set; }
        public string? CropName { get; set; }
        public string? SectionName { get; set; }
        public IFormFile? ImageFile { get; set; }

        public List<UhsbImageFileGridVM>? Files { get; set; }
    }

    public class CropGridVM
    {
        public int CropId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? filename { get; set; }

        public IFormFile ImageFile { get; set; }

        public List<UhsbImageFileGridVM> Files { get; set; }

    }



    public class UhsbImageFileGridVM
    {
        public int FileId { get; set; }

        public int? ItemId { get; set; }

        public int? FileType { get; set; }

        public string? FilePath { get; set; }
    }

    public class UhsbSectionCreateUpdateVM
    {
        public int CropId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class DeleteItemVM
    {
        public int CategoryId { get; set; } = 0;
        public int CropId { get; set; } = 0;
        public int ItemDetailId { get; set; } = 0;
        public CommonEnum.FileTypes PageType { get; set; } = CommonEnum.FileTypes.Crops;
    }

}
