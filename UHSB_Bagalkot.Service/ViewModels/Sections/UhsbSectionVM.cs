using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public class UhsbSectionCreateUpdateVM
    {
        public int CropId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
