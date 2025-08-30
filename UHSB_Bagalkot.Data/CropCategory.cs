using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Data
{
    public class CropCategory
    {
        [Key]   // <-- This tells EF Core that this is the PK
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }

        public ICollection<Crop> Crops { get; set; }
    }
}
