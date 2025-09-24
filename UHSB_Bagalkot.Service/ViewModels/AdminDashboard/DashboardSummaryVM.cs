using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.ViewModels.AdminDashboard
{
    public class DashboardSummaryVM
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int Farmers { get; set; }
        public int Categories { get; set; }
        public int Crops { get; set; }
        public int WeatherFiles { get; set; }
    }
}
