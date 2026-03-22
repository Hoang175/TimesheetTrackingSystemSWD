using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.DAL.Repositories
{
    public class TimesheetRepository : ITimesheetRepository
    {
        private readonly TimesheetTrackingSystemSwdContext _context;

        public TimesheetRepository(TimesheetTrackingSystemSwdContext context)
        {
            _context = context;
        }

        public async Task<Timesheet?> GetTimesheetByEmployeeAndPeriodAsync(int employeeId, DateOnly startDate, DateOnly endDate)
        {
            return await _context.Timesheets
                .FirstOrDefaultAsync(t => t.EmployeeId == employeeId && t.PeriodStart == startDate && t.PeriodEnd == endDate && t.IsDeleted == false);
        }

        public async Task<IEnumerable<Timesheet>> GetTimesheetsByEmployeeIdAsync(int employeeId)
        {
            return await _context.Timesheets
                .Where(t => t.EmployeeId == employeeId && t.IsDeleted == false)
                .OrderByDescending(t => t.PeriodStart)
                .ToListAsync();
        }

        public async Task<Timesheet> CreateTimesheetAsync(Timesheet timesheet)
        {
            _context.Timesheets.Add(timesheet);
            await _context.SaveChangesAsync();
            return timesheet;
        }

        public async Task UpdateTimesheetAsync(Timesheet timesheet)
        {
            _context.Timesheets.Update(timesheet);
            await _context.SaveChangesAsync();
        }

        public async Task<Employee?> GetEmployeeByUserIdAsync(int userId)
        {
            return await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.IsDeleted == false);
        }

        public async Task<IEnumerable<Timesheet>> GetPendingTimesheetsAsync()
        {
            return await _context.Timesheets
                .Include(t => t.Employee)
                .Where(t => t.Status == "Pending" && t.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<int> CountByStatusAsync(string status)
        {
            return await _context.Timesheets
                .Where(t => t.Status == status && t.IsDeleted != true)
                .CountAsync();
        }

        public async Task<Timesheet?> GetByIdAsync(int timesheetId)
        {
            return await _context.Timesheets
                .FirstOrDefaultAsync(t => t.TimesheetId == timesheetId);
        }
    }
}
