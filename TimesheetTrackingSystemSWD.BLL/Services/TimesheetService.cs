using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimesheetTrackingSystemSWD.BLL.DTOs;
using TimesheetTrackingSystemSWD.BLL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.BLL.Services
{
    public class TimesheetService : ITimesheetService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly ISystemLogRepository _logRepository;
        private readonly TimesheetTrackingSystemSwdContext _context;

        public TimesheetService(
            IAttendanceRepository attendanceRepository,
            ITimesheetRepository timesheetRepository,
            ISystemLogRepository logRepository,
            TimesheetTrackingSystemSwdContext context)
        {
            _attendanceRepository = attendanceRepository;
            _timesheetRepository = timesheetRepository;
            _logRepository = logRepository;
            _context = context;
        }

        public async Task<Employee?> GetEmployeeProfileAsync(int userId)
        {
            return await _timesheetRepository.GetEmployeeByUserIdAsync(userId);
        }

        public async Task<TimesheetSubmissionDTO> GetCurrentTimesheetSubmissionDataAsync(int userId)
        {
            var employee = await _timesheetRepository.GetEmployeeByUserIdAsync(userId);
            if (employee == null) return null;

            var today = DateTime.Today;
            DateOnly periodStart, periodEnd;
            string periodType = "Weekly";

            // Determine if we should show Monthly or Weekly based on current date
            if (IsEndOfMonth(today))
            {
                periodStart = new DateOnly(today.Year, today.Month, 1);
                periodEnd = periodStart.AddMonths(1).AddDays(-1);
                periodType = "Monthly";
            }
            else
            {
                // Current week: Monday to Sunday
                int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                var monday = today.AddDays(-1 * diff).Date;
                periodStart = DateOnly.FromDateTime(monday);
                periodEnd = periodStart.AddDays(6);
            }

            var attendanceRecords = await _attendanceRepository.GetAttendanceRecordsByEmployeeIdAndPeriodAsync(employee.EmployeeId, periodStart, periodEnd);
            var existingTimesheet = await _timesheetRepository.GetTimesheetByEmployeeAndPeriodAsync(employee.EmployeeId, periodStart, periodEnd);

            var dtoList = attendanceRecords.Select(a => new AttendanceRecordDTO
            {
                AttendanceId = a.AttendanceId,
                WorkDate = a.WorkDate,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                DailyHours = CalculateHours(a.CheckInTime, a.CheckOutTime),
                IsComplete = a.CheckInTime.HasValue && a.CheckOutTime.HasValue
            }).ToList();

            decimal totalHours = dtoList.Sum(a => a.DailyHours ?? 0);

            // Validation logic
            bool canSubmit = true;
            string? validationMessage = null;

            if (existingTimesheet != null)
            {
                canSubmit = false;
                validationMessage = "Bạn đã nộp timesheet cho kỳ này rồi.";
            }
            else if (!IsSubmissionTime(today))
            {
                canSubmit = false;
                validationMessage = "Chưa đến thời điểm nộp timesheet (Thứ 6 - Chủ Nhật hoặc cuối tháng).";
            }
            else if (dtoList.Count == 0)
            {
                canSubmit = false;
                validationMessage = "Không có dữ liệu điểm danh cho kỳ này.";
            }
            else if (dtoList.Any(a => !a.IsComplete))
            {
                canSubmit = false;
                validationMessage = "Vui lòng hoàn tất tất cả dữ liệu Check-in/Check-out trước khi nộp.";
            }

            return new TimesheetSubmissionDTO
            {
                EmployeeId = employee.EmployeeId,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                PeriodType = periodType,
                TotalWorkingHours = totalHours,
                AttendanceRecords = dtoList,
                CanSubmit = canSubmit,
                ValidationMessage = validationMessage
            };
        }

        public async Task<bool> SubmitTimesheetAsync(int userId, DateOnly periodStart, DateOnly periodEnd)
        {
            var employee = await _timesheetRepository.GetEmployeeByUserIdAsync(userId);
            if (employee == null) return false;

            // Start Transaction to ensure data integrity
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Re-validate
                    var existing = await _timesheetRepository.GetTimesheetByEmployeeAndPeriodAsync(employee.EmployeeId, periodStart, periodEnd);
                    if (existing != null) return false;

                    var records = await _attendanceRepository.GetAttendanceRecordsByEmployeeIdAndPeriodAsync(employee.EmployeeId, periodStart, periodEnd);
                    if (!records.Any() || records.Any(r => r.CheckInTime == null || r.CheckOutTime == null)) return false;

                    decimal totalHours = records.Sum(r => CalculateHours(r.CheckInTime, r.CheckOutTime) ?? 0);

                    // 2. Create Timesheet
                    var timesheet = new Timesheet
                    {
                        EmployeeId = employee.EmployeeId,
                        PeriodStart = periodStart,
                        PeriodEnd = periodEnd,
                        TotalWorkingHours = totalHours,
                        Status = "Pending",
                        CreatedAt = DateTime.Now
                    };

                    await _timesheetRepository.CreateTimesheetAsync(timesheet);

                    // 3. Create Audit Log
                    var log = new SystemLog
                    {
                        UserId = userId,
                        Action = $"Submitted timesheet for period {periodStart:dd/MM/yyyy} - {periodEnd:dd/MM/yyyy}. Total hours: {totalHours}",
                        Timestamp = DateTime.Now,
                        // Avoid navigation property issues by not setting the User object explicitly if not needed, 
                        // but normally EF handles just the UserId.
                    };
                    await _logRepository.CreateLogAsync(log);

                    // Commit Transaction
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        private bool IsEndOfMonth(DateTime date)
        {
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            return date.Day >= daysInMonth - 2; // Last 3 days of the month
        }

        private bool IsSubmissionTime(DateTime date)
        {
            // Friday (5), Saturday (6), Sunday (0)
            bool isWeekend = date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
            return isWeekend || IsEndOfMonth(date);
        }

        private decimal? CalculateHours(DateTime? checkIn, DateTime? checkOut)
        {
            if (checkIn == null || checkOut == null) return null;
            var duration = checkOut.Value - checkIn.Value;
            return (decimal)Math.Round(duration.TotalHours, 2);
        }
    }
}
