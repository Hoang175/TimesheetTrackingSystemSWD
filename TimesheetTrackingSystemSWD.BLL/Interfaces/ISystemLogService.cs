using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.BLL.Interfaces
{
    public interface ISystemLogService
    {
        Task<IEnumerable<SystemLog>> GetTimesheetLogsAsync();
    }
}
