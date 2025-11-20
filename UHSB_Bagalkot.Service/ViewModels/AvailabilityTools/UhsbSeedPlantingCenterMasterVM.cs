using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.ViewModels.AvailabilityTools
{
    public class UhsbSeedPlantingCenterMasterVM
    {
        public int CenterId { get; set; }

        public int? DistrictId { get; set; }

        public string? CenternameEng { get; set; }

        public string? CenternameKnd { get; set; }
    }
}
