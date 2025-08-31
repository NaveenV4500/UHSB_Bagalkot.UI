using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Data
{
    public class CropDetail
    {
        public int DetailId { get; set; }
        public int CropId { get; set; }
        public string DetailType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public Crop Crop { get; set; }
    }
}
