using System;
using System.Threading.Tasks;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.DAL.Interfaces
{
    public interface ISystemLogRepository
    {
        Task CreateLogAsync(SystemLog log);
    }
}
