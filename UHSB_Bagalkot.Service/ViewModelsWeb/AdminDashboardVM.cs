using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.WebService.ViewModels
{
    public class AdminDashboardVM
    {
        public SummaryVM Summary { get; set; }
        public List<FarmersByVillageVM> FarmersByVillage { get; set; }
        public WeeklyWeatherVM WeeklyWeather { get; set; }
    }
    public class FarmersByVillageVM
    {
        public string Village { get; set; }
        public int Count { get; set; }
    }

    public class SummaryVM
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int Farmers { get; set; }
        public int Categories { get; set; }
        public int Crops { get; set; }
        public int WeatherFiles { get; set; }
    }
     

    public class ForecastVM
    {
        public string Date { get; set; }
        public double Temperature { get; set; }
        public string Condition { get; set; }
    }

    public class WeeklyWeatherVM
    {
        public int weatherFileId { get; set; }
        public int userId { get; set; }
        public string weekStartDate { get; set; }
        public string weekEndDate { get; set; }
        public string filePath { get; set; }
        public string description { get; set; }
        public string createdDate { get; set; }
        public int districtId { get; set; }
        public object district { get; set; }
        public object user { get; set; }
    }
    
}
