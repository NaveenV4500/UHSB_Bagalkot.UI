using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Data.Models
{
    public partial class uhsbSectionsMapping
    {
        [Key]
        public int SectionMapId { get; set; }

        public int SectionId { get; set; }

        public int CropId { get; set; }
    }
}
