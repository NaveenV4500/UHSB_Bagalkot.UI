using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_MasterService.ViewModels
{
    public class FilterGridModel
    {
        public int filterBy { get; set; }
        public Dictionary<byte, string> SearchText { get; set; }
        public Common.GridEnum.DataTypeEnum DataType { get; set; }
        public Common.GridEnum.FilterTypeEnum Contain { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? GenralDate { get; set; }
    }
}
