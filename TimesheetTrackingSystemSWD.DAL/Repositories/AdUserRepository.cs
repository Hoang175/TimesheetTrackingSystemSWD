using Microsoft.EntityFrameworkCore;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.DAL.Repositories
{
    public class AdUserRepository : IAdUserRepository
    {
        private readonly TimesheetTrackingSystemSwdContext _context;

        public AdUserRepository(TimesheetTrackingSystemSwdContext context)
        {
            _context = context;
        }

        public IQueryable<User> GetAllUsers(bool isDeleted = false)
        {
            return _context.Users
                .Include(u => u.Role)
                .Include(u => u.Employee)
                .Where(u => u.IsDeleted == isDeleted);
        }

        public User? GetUserById(int id, bool isDeleted = false)
        {
            return _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.UserId == id && u.IsDeleted == isDeleted);
        }

        public List<Role> GetAllRoles()
        {
            return _context.Roles.Where(r => r.IsDeleted == false).ToList();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}