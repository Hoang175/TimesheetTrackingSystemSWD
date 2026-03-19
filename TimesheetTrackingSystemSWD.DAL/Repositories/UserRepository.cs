using Microsoft.EntityFrameworkCore;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;


// @HoangDH
namespace TimesheetTrackingSystemSWD.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TimesheetTrackingSystemSwdContext _context;

        public UserRepository(TimesheetTrackingSystemSwdContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Role) 
                .Include(u => u.Employee) 
                .FirstOrDefaultAsync(u => u.Username == username && u.IsDeleted == false);
        }
    }
}