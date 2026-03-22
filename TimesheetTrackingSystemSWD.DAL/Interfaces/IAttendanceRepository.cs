using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.DAL.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<AttendanceRecord>> GetAttendanceRecordsByEmployeeIdAndPeriodAsync(int employeeId, DateOnly startDate, DateOnly endDate);
        Task<AttendanceRecord?> GetAttendanceRecordByIdAsync(int attendanceId);
        Task UpdateAttendanceRecordAsync(AttendanceRecord record);
        Task AddAttendanceRecordAsync(AttendanceRecord record);
    }
}
