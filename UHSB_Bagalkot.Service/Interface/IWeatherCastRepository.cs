using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface IWeatherCastRepository
    {
        Task<IEnumerable<UhsbWeatherCastFileDetail>> GetAllAsync();
        Task<UhsbWeatherCastFileDetail?> GetByIdAsync(int id);
        bool SaveFileAsync(WeatherFileUploadVM dto);
        Task<List<WeeklyWeatherReportGridVM>> GetWeeklyReportsAsync(int districtId);

    }
}
