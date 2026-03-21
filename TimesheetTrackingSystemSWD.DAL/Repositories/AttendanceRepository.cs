using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.DAL.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly TimesheetTrackingSystemSwdContext _context;

        public AttendanceRepository(TimesheetTrackingSystemSwdContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttendanceRecord>> GetAttendanceRecordsByEmployeeIdAndPeriodAsync(int employeeId, DateOnly startDate, DateOnly endDate)
        {
            return await _context.AttendanceRecords
                .Where(a => a.EmployeeId == employeeId && a.WorkDate >= startDate && a.WorkDate <= endDate && a.IsDeleted == false)
                .OrderBy(a => a.WorkDate)
                .ToListAsync();
        }

        public async Task<AttendanceRecord?> GetAttendanceRecordByIdAsync(int attendanceId)
        {
            return await _context.AttendanceRecords.FirstOrDefaultAsync(a => a.AttendanceId == attendanceId && a.IsDeleted == false);
        }

        public async Task UpdateAttendanceRecordAsync(AttendanceRecord record)
        {
            _context.AttendanceRecords.Update(record);
            await _context.SaveChangesAsync();
        }
    }
}
