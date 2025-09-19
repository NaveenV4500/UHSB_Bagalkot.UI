using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class WeatherCastRepository : CommonConnection, IWeatherCastRepository
    {
        private readonly IWeatherCastRepository _repository;

        public WeatherCastRepository(Uhsb2025Context context) : base(context)
        {

        }
        public async Task<IEnumerable<UhsbWeatherCastFileDetail>> GetAllAsync()
        {
            return await _context.UhsbWeatherCastFileDetails.ToListAsync();
        }

        public async Task<UhsbWeatherCastFileDetail?> GetByIdAsync(int id)
        {
            return await _context.UhsbWeatherCastFileDetails
                                 .FirstOrDefaultAsync(x => x.WeatherFileId == id);
        }




        public bool SaveFileAsync(WeatherFileUploadVM dto)
        {
            var fullPath = CommonHelper.GetFilePath("UploadedWeatherFiles", dto.FileName+".pdf");

            // Save file asynchronously
            CommonHelper.SaveFileAsync(fullPath, dto.FileBytes);

            // Calculate start and end of week
            var startOfWeek = StartOfWeek(DateTime.Now, DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(6);

            var entity = new UhsbWeatherCastFileDetail
            {
                UserId = dto.UserId,
                DistrictId = dto.DistrictId,
                FilePath = fullPath,
                Description = dto.Description,
                WeekStartDate = DateOnly.FromDateTime(startOfWeek),
                WeekEndDate = DateOnly.FromDateTime(endOfWeek),
                CreatedDate = DateOnly.FromDateTime(DateTime.Now)
            };


            _context.UhsbWeatherCastFileDetails.Add(entity);
            _context.SaveChangesAsync();

            return true;
        }

        public static DateTime StartOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
            return date.Date.AddDays(-diff);
        }



        //public async Task<List<SPWeeklyWeatherRecord>> GetWeeklyReportsAsync(int districtId)
        //{
        //    var param = new SqlParameter("@DistrictId", districtId);

        //    using (var connection = _context.Database.GetDbConnection())
        //    {
        //        var results = await connection.QueryAsync<SPWeeklyWeatherRecord>(
        //            "EXEC dbo.sp_GetWeeklyWeatherRecords @DistrictId", new { DistrictId = districtId });
        //        return results.ToList();
        //    }
        //}

        public async Task<List<WeeklyWeatherReportGridVM>> GetWeeklyReportsAsync(int districtId)
        {
            var today = DateTime.Today;

            var diffToMonday = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var currentWeekStart = DateOnly.FromDateTime(today.AddDays(-diffToMonday));
            var currentWeekEnd = currentWeekStart.AddDays(6);

            var prevWeekStart = currentWeekStart.AddDays(-7);
            var prevWeekEnd = currentWeekEnd.AddDays(-7);

            var query =
                from r in _context.UhsbWeatherCastFileDetails
                where r.DistrictId == districtId &&
                      ((r.WeekStartDate <= currentWeekEnd && r.WeekEndDate >= currentWeekStart) ||
                       (r.WeekStartDate <= prevWeekEnd && r.WeekEndDate >= prevWeekStart))
                let weekType =
                    (r.WeekStartDate <= currentWeekEnd && r.WeekEndDate >= currentWeekStart)
                        ? "CurrentWeek"
                        : (r.WeekStartDate <= prevWeekEnd && r.WeekEndDate >= prevWeekStart)
                            ? "PreviousWeek"
                            : "Other"
                orderby r.WeekStartDate descending, r.CreatedDate descending
                select new
                {
                    r,
                    weekType
                };

            var result = await query
                .GroupBy(x => new { x.r.DistrictId, x.r.WeekStartDate })
                .Select(g => new WeeklyWeatherReportGridVM
                {
                    WeatherFileId = g.First().r.WeatherFileId,
                    DistrictId = g.First().r.DistrictId ?? 0,
                    WeekStartDate = g.First().r.WeekStartDate.Value,
                    WeekEndDate = g.First().r.WeekEndDate.Value,
                    CreatedDate = g.First().r.CreatedDate,
                    FilePath = g.First().r.FilePath, 
                })
                .ToListAsync();

            return result;
        }
         
    }
}



