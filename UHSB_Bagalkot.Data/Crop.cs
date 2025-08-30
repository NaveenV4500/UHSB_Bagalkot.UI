using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Data
{
    public class Crop
    {
        public int CropId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? PestSolutions { get; set; }
        public DateTime CreatedDate { get; set; }

        public CropCategory Category { get; set; }
    }
}
