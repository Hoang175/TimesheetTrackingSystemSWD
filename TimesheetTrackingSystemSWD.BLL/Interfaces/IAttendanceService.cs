using System;
using System.Threading.Tasks;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.BLL.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceRecord?> GetTodayAttendanceAsync(int userId);
        Task<bool> CheckInAsync(int userId);
        Task<bool> CheckOutAsync(int userId);
    }
}
