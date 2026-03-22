using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.DAL.Interfaces
{
    public interface ITimesheetRepository
    {
        Task<Timesheet?> GetTimesheetByEmployeeAndPeriodAsync(int employeeId, DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<Timesheet>> GetTimesheetsByEmployeeIdAsync(int employeeId);
        Task<Timesheet> CreateTimesheetAsync(Timesheet timesheet);
        Task UpdateTimesheetAsync(Timesheet timesheet);
        Task<Employee?> GetEmployeeByUserIdAsync(int userId);
    }
}
