using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.WebService.ViewModels
{
    public class LoginResponse
    {
        public bool success { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public string userName { get; set; }
        public string userRoleType { get; set; }
        public string phoneNo { get; set; }
        public int userID { get; set; }
        public int userCount { get; set; }
    }
}
