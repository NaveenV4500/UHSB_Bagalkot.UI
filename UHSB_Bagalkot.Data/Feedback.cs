using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Data
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public int? CropId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserMaster User { get; set; }
        public Crop Crop { get; set; }
    }
}
