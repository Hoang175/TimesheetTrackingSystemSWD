using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimesheetTrackingSystemSWD.BLL.DTOs;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.BLL.Interfaces
{
    public interface ITimesheetService
    {
        Task<TimesheetSubmissionDTO> GetCurrentTimesheetSubmissionDataAsync(int userId);
        Task<bool> SubmitTimesheetAsync(int userId, DateOnly periodStart, DateOnly periodEnd);
        Task<Employee?> GetEmployeeProfileAsync(int userId);
    }
}
