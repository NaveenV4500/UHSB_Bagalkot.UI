using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Data
{
    public class Crop
    {
        public int CropId { get; set; }
        public int CategoryId { get; set; }
        public string CropName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public CropCategory Category { get; set; }
        public ICollection<CropDetail> CropDetails { get; set; }
        public ICollection<Article> Articles { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
