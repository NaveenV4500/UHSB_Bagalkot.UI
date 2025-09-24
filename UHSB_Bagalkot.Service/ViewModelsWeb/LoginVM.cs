using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.WebService.ViewModels
{
    public class LoginVM
    {
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsFromadmin { get; set; }
    }
}
