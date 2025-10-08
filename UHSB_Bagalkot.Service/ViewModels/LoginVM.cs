using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.ViewModels
{
    public class LoginVM
    {
        public string UserName { get; set; }

        public string PhoneNumber { get; set; }
        public bool IsFromadmin { get; set; }
    }
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
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
