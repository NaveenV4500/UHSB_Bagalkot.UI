using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Data
{
    [Table("FarmersProfile")]
    public class FarmerProfile
    {
        [Key]
        public int FarmerId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string? Village { get; set; }
        public decimal? LandSize { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
