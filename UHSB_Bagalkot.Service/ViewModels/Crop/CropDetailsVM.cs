using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.ViewModels.Crop
{
    public class CropDetailsVM
    {
        public int CropId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
    public class DropdownItemCropVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RequestItemDetailsVM
    {
        public int ItemId { get; set; }

        public int SubSectionId { get; set; }

        public string? Name { get; set; }

        public string? ImageUrl { get; set; }

        public int SectionMapId { get; set; }

        public int CropId { get; set; }

        public int CategoryId { get; set; }

        public int SectionId { get; set; }
    }

    public class RequestSectionDetailsVM
    {
        public int SectionId { get; set; }

        public int CropId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
        public string? filename { get; set; }
        
    }
    public class RequestCategoryDetailsVM
    {
        public int CategoryId { get; set; }

        public string? Name { get; set; }

        public string? ImageUrl { get; set; }
    }

    
    }
