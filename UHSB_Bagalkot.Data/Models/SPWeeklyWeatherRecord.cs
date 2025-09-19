using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Data.Models
{
    public partial class SPWeeklyWeatherRecord
    {
        public long Id { get; set; }                // bigint in SQL → long
        public int DistrictId { get; set; }        // bigint → long
        public string FilePath { get; set; }
        public string Description { get; set; }
        public DateOnly? WeekStartDate { get; set; } // date → DateOnly
        public DateOnly? WeekEndDate { get; set; }   // date → DateOnly
        public DateOnly? CreatedDate { get; set; }   // date → DateOnly
        public string WeekType { get; set; }


    }
}
