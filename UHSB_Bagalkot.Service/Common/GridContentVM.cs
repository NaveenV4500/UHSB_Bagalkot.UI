using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace UHSB_Bagalkot.Service.Common
{
    public class GridContentVM
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public GridEnum.FTPDocumentsLogs OrderBy { get; set; } = GridEnum.FTPDocumentsLogs.BranchName;
        public bool IsDescending { get; set; } = false;
        public string FilterDetails { get; set; }
        public string ExternalFilter { get; set; }
        //public int SubSectId { get; set; } = 0;
    }
}
