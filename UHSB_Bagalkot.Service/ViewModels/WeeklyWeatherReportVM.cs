using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.ViewModels
{
    public class WeeklyWeatherReportFilesVM
    {
        public int Id { get; set; }
        public int FileType { get; set; } 
        public string FileByteArry { get; set; }
    }
    public class WeeklyWeatherReportGridVM 
    {
        public int WeatherFileId { get; set; }                
        public int DistrictId { get; set; }        
        public string FilePath { get; set; }
        public string Description { get; set; }
        public DateOnly? WeekStartDate { get; set; }  
        public DateOnly? WeekEndDate { get; set; }    
        public DateOnly? CreatedDate { get; set; }   
        public string WeekType { get; set; }
    }
}
