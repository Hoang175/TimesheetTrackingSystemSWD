using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.DAL.Interfaces
{
    public interface IAdEmployeeRepository
    {
        //IQueryable<Employee> GetAllEmployees();
        //Employee? GetEmployeeById(int id);
        IQueryable<Employee> GetAllEmployees(bool isDeleted = false);
        Employee? GetEmployeeById(int id, bool isDeleted = false);
        string? GetLastEmployeeCode(string prefix);
        bool CheckEmailExists(string email);
        int GetRoleIdByName(string roleName); // Dùng để gán quyền
        void AddEmployee(Employee emp);
        void UpdateEmployee(Employee emp);
        void Save();
    }
}