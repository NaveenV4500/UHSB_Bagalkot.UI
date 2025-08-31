using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Data
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        public int? CropId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserMaster User { get; set; }
        public Crop Crop { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}
