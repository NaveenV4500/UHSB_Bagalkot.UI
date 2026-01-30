using Microsoft.AspNetCore.Http;
using OpenQA.Selenium.BiDi.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Service.Interface;

namespace UHSB_Bagalkot.Service.Common
{
    public class CurrentUser : ICurrentUser
    {
        private readonly ClaimsPrincipal _user;

        public int UserId { get; }
        public string UserName { get; }
        public string Role { get; } 

        public CurrentUser(IHttpContextAccessor accessor)
        {
            _user = accessor.HttpContext?.User;
            UserId = int.Parse(_user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            UserName = _user?.FindFirst(ClaimTypes.Name)?.Value;
            Role = _user?.FindFirst(ClaimTypes.Role)?.Value; 
        }
    }
}
