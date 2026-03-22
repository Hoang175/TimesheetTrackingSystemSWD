using System;
using System.Threading.Tasks;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.BLL.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceRecord?> GetTodayAttendanceAsync(int userId);

        Task<bool> CheckInAsync(int userId, string ipAddress);
        Task<bool> CheckOutAsync(int userId, string ipAddress);
        Task<IEnumerable<AttendanceRecord>> GetMonthlyAttendanceAsync(int userId, int year, int month);

    }
}
