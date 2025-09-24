using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.WebService.ViewModels.CategoryModels
{
    public class CategoryVM
    { 
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public string? ImageUrl { get; set; }
            //public ICollection<CropVM>? Crops { get; set; }
        } 
}
