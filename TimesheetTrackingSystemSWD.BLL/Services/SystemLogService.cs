using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimesheetTrackingSystemSWD.BLL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.BLL.Services
{
    public class SystemLogService : ISystemLogService
    {
        private readonly ISystemLogRepository _logRepository;

        public SystemLogService(ISystemLogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<IEnumerable<SystemLog>> GetTimesheetLogsAsync()
        {
            return await _logRepository.GetTimesheetLogsAsync();
        }
    }
}
