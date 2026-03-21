using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimesheetTrackingSystemSWD.BLL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.BLL.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly TimesheetTrackingSystemSwdContext _context;

        public AttendanceService(
            IAttendanceRepository attendanceRepository,
            ITimesheetRepository timesheetRepository,
            TimesheetTrackingSystemSwdContext context)
        {
            _attendanceRepository = attendanceRepository;
            _timesheetRepository = timesheetRepository;
            _context = context;
        }

        public async Task<AttendanceRecord?> GetTodayAttendanceAsync(int userId)
        {
            var employee = await _timesheetRepository.GetEmployeeByUserIdAsync(userId);
            if (employee == null) return null;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var records = await _attendanceRepository.GetAttendanceRecordsByEmployeeIdAndPeriodAsync(employee.EmployeeId, today, today);
            return records.FirstOrDefault();
        }

        public async Task<bool> CheckInAsync(int userId)
        {
            var employee = await _timesheetRepository.GetEmployeeByUserIdAsync(userId);
            if (employee == null) return false;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var records = await _attendanceRepository.GetAttendanceRecordsByEmployeeIdAndPeriodAsync(employee.EmployeeId, today, today);
            if (records.Any()) return false; // Already checked in today

            var record = new AttendanceRecord
            {
                EmployeeId = employee.EmployeeId,
                WorkDate = today,
                CheckInTime = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            _context.AttendanceRecords.Add(record);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckOutAsync(int userId)
        {
            var employee = await _timesheetRepository.GetEmployeeByUserIdAsync(userId);
            if (employee == null) return false;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var records = await _attendanceRepository.GetAttendanceRecordsByEmployeeIdAndPeriodAsync(employee.EmployeeId, today, today);
            var record = records.FirstOrDefault();
            
            if (record == null || record.CheckInTime == null || record.CheckOutTime != null) return false;

            record.CheckOutTime = DateTime.Now;
            record.UpdatedAt = DateTime.Now;
            
            _context.AttendanceRecords.Update(record);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
