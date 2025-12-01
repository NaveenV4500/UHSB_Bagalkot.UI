using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static UHSB_Bagalkot.Service.Common.CommonEnum;

namespace UHSB_Bagalkot.Service.ViewModels.AvailabilityTools
{
    public class AvailabilityToolsDetailsVM
    {
        public int Identifier { get; set; }
        public int CenterId { get; set; }

        public string Centername_eng { get; set; }
        public string Centername_knd { get; set; }

        public int DistrictId { get; set; }

        public int HeadId { get; set; }
        public string RecordHead_eng { get; set; }
        public string RecordHead_knd { get; set; }

        public string AvailToolname_eng { get; set; }
        public string AvailToolname_knd { get; set; }

        public int? Quantity { get; set; }
        public string Unit { get; set; }
        public string UnitName { get; set; }
        public UnitType? UnitEnum
        {
            get
            {
                if (string.IsNullOrEmpty(UnitName))
                    return null;

                return (UnitType)Enum.Parse(typeof(UnitType), UnitName);
            }
            set
            {
                UnitName = value?.ToString();
            }
        }

        public decimal? Price { get; set; }

        public DateTime? AvailabilityDate { get; set; }
        public string Remarks { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        
        public IEnumerable<SelectListItem> DistrictType { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Centerstype { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Centervariatestype { get; set; } = new List<SelectListItem>();
    }

    public class CentersAvailabilityDetailsVM
    {
        public string TempFilePath { get; set; }
        public string OriginalFileName { get; set; }
        public List<AvailabilityToolsDetailsVM> items { get; set; } = new List<AvailabilityToolsDetailsVM>();
        public int CenterId { get; set; }
        public int HeadId { get; set; } 
        public IEnumerable<SelectListItem> Centerstype { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Centervariatestype { get; set; } = new List<SelectListItem>(); 

    }
 
}
