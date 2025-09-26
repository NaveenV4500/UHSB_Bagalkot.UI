using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.WebService.AppSettings
{
     
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
         
        public string MicrosoftAspNetCore { get; set; }
    }

    public class ApiSettings
    {
        public Logging Logging { get; set; }
        public UploadSettings UploadSettings { get; set; }
        public string AllowedHosts { get; set; }
        public string Base_Url { get; set; }
    }

    public class UploadSettings
    {
        public string TempFilesPath { get; set; }
    }

}
