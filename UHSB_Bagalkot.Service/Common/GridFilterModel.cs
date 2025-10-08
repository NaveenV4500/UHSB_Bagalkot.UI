using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.Common
{
    public class GridFilterModel
    {
        public byte filterBy { get; set; }
        public string filterTxt { get; set; }
        public byte filterType { get; set; } 
    }
    public class GridFilterChildGridModel
    {
        public byte filterBy_ChildGrid { get; set; }
        public string filterTxt_ChildGrid { get; set; }
        public byte filterType_ChildGrid { get; set; } 
    }
}
