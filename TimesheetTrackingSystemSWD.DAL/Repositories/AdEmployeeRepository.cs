using Microsoft.EntityFrameworkCore;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.DAL.Repositories
{
    public class AdEmployeeRepository : IAdEmployeeRepository
    {
        private readonly TimesheetTrackingSystemSwdContext _context;

        public AdEmployeeRepository(TimesheetTrackingSystemSwdContext context)
        {
            _context = context;
        }

        //public IQueryable<Employee> GetAllEmployees()
        //{
        //    return _context.Employees
        //        .Include(e => e.User)
        //        .Include(e => e.Department)
        //        .Where(e => e.IsDeleted == false);
        //}

        //public Employee? GetEmployeeById(int id)
        //{
        //    return _context.Employees
        //        .Include(e => e.User)
        //        .FirstOrDefault(e => e.EmployeeId == id && e.IsDeleted == false);
        //}
        public IQueryable<Employee> GetAllEmployees(bool isDeleted = false)
        {
            return _context.Employees
                .Include(e => e.User)
                .Include(e => e.Department)
                .Where(e => e.IsDeleted == isDeleted); // Lọc theo cờ truyền vào
        }

        public Employee? GetEmployeeById(int id, bool isDeleted = false)
        {
            return _context.Employees
                .Include(e => e.User)
                .FirstOrDefault(e => e.EmployeeId == id && e.IsDeleted == isDeleted);
        }


        public string? GetLastEmployeeCode(string prefix)
        {
            return _context.Employees
                .Where(e => e.EmployeeCode.StartsWith(prefix))
                .OrderByDescending(e => e.EmployeeCode)
                .Select(e => e.EmployeeCode)
                .FirstOrDefault();
        }

        public bool CheckEmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email && u.IsDeleted == false);
        }

        public int GetRoleIdByName(string roleName)
        {
            var role = _context.Roles.FirstOrDefault(r => r.RoleName == roleName);
            return role != null ? role.RoleId : 0;
        }

        public void AddEmployee(Employee emp)
        {
            _context.Employees.Add(emp);
        }

        public void UpdateEmployee(Employee emp)
        {
            _context.Employees.Update(emp);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}