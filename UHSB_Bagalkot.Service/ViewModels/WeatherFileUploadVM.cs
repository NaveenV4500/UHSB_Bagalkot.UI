using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.ViewModels
{
    public class WeatherFileUploadVM
    {
        public int UserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public byte[] FileBytes { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public int? DistrictId { get; set; }
    }
}
