using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Data
{
    [Table("UserRoles")]
    public class UserRoles
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RoleName { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserRoles> UserRole { get; set; }
    }
}
