using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Data
{
    public class Article
    {
        public int ArticleId { get; set; }
        public int? CropId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public Crop Crop { get; set; }
        public UserMaster CreatedByUser { get; set; }
    }
}
