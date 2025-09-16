using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.ViewModels.CropProfile
{ 
    public   class UhsbItemQnAVM
    {
        public int QnAid { get; set; }

        public int ItemId { get; set; }

        public string Question { get; set; } = null!;

        public string Answer { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }
        public byte? Datastatus { get; set; }

        public int? UserID { get; set; }

        public string? ImageUrl { get; set; }
    }
}
