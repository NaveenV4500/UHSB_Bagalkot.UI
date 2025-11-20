using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UHSB_Bagalkot.Data.Models;

namespace UHSB_Bagalkot.Service.ViewModels.AdminDashboard
{
    public class CropContentWithImageVM
    {
        public string TempFilePath { get; set; }
        public string OriginalFileName { get; set; }

        public List<CropContentItems> ItemImages { get; set; } = new List<CropContentItems>();
        public int CategoryId { get; set; }
        public int CropsId { get; set; }
        public int SectionsId { get; set; }
        public int SubSectionsId { get; set; }
        public int ItemsId { get; set; }
        public int ImageId { get; set; }
        
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Crops { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Sections { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> SubSections { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Items { get; set; } = new List<SelectListItem>();

    }

    public class CropContentItems
    {
        public int? ItemId { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public IFormFile ImageFile { get; set; }
        public string ImageBase64 { get; set; }
        public string ImageName { get; set; }
    }
    public class UhsbItemImageVM
    {
        public int? ImageId { get; set; }
        public int? ItemId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string CropName { get; set; } = string.Empty;
        public int? CropId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int? SectionId { get; set; }
    }

}
