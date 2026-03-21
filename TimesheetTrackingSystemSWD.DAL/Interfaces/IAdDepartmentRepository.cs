using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.DAL.Interfaces
{
    public interface IAdDepartmentRepository
    {
        IQueryable<Department> GetAllDepartments(bool isDeleted = false);
        Department? GetDeletedDepartmentById(int id);
        Department? GetDepartmentById(int id);
        bool CheckNameExists(string name, int? excludeId = null);
        void AddDepartment(Department dept);
        void UpdateDepartment(Department dept);
        void Save();
    }
}