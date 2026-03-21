using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.DAL.Interfaces
{
    public interface IAdUserRepository
    {
        IQueryable<User> GetAllUsers(bool isDeleted = false);
        User? GetUserById(int id, bool isDeleted = false);
        List<Role> GetAllRoles();
        void UpdateUser(User user);
        void Save();
    }
}