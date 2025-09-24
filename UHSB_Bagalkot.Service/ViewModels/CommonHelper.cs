using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.ViewModels
{
    public static class CommonHelper
    {
         

        public static async Task SaveFileAsync(string fullPath, byte[] fileBytes)
        { 
            await File.WriteAllBytesAsync(fullPath, fileBytes);
        }


        public static (DateTime startOfWeek, DateTime endOfWeek) GetWeekRange(DateTime date)
        {
            // Assuming week starts on Monday
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = date.AddDays(-1 * diff).Date;
            DateTime endOfWeek = startOfWeek.AddDays(6).Date;
            return (startOfWeek, endOfWeek);
        }

        public static (DateTime startOfPreviousWeek, DateTime endOfPreviousWeek) GetPreviousWeekRange(DateTime date)
        {
            var currentWeek = GetWeekRange(date);
            DateTime startPrev = currentWeek.startOfWeek.AddDays(-7);
            DateTime endPrev = currentWeek.startOfWeek.AddDays(-1);
            return (startPrev, endPrev);
        }
    }
}
