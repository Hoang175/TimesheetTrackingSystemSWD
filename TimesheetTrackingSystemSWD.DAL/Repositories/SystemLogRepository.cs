using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.DAL.Repositories
{
    public class SystemLogRepository : ISystemLogRepository
    {
        private readonly TimesheetTrackingSystemSwdContext _context;

        public SystemLogRepository(TimesheetTrackingSystemSwdContext context)
        {
            _context = context;
        }

        public async Task CreateLogAsync(SystemLog log)
        {
            _context.SystemLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
