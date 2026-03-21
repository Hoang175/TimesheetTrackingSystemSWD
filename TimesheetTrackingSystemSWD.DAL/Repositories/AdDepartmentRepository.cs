using Microsoft.EntityFrameworkCore;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.DAL.Repositories
{
    public class AdDepartmentRepository : IAdDepartmentRepository
    {
        private readonly TimesheetTrackingSystemSwdContext _context;

        public AdDepartmentRepository(TimesheetTrackingSystemSwdContext context)
        {
            _context = context;
        }


        public IQueryable<Department> GetAllDepartments(bool isDeleted = false)
        {
            return _context.Departments
                .Include(d => d.Employees.Where(e => e.IsDeleted == false))
                .Where(d => d.IsDeleted == isDeleted); 
        }

        public Department? GetDeletedDepartmentById(int id)
        {
            return _context.Departments.FirstOrDefault(d => d.DepartmentId == id && d.IsDeleted == true);
        }

        public Department? GetDepartmentById(int id)
        {
            return _context.Departments
                .Include(d => d.Employees.Where(e => e.IsDeleted == false))
                .FirstOrDefault(d => d.DepartmentId == id && d.IsDeleted == false);
        }

        public bool CheckNameExists(string name, int? excludeId = null)
        {
            var query = _context.Departments.Where(d => d.DepartmentName.ToLower() == name.ToLower() && d.IsDeleted == false);
            if (excludeId.HasValue)
            {
                query = query.Where(d => d.DepartmentId != excludeId.Value);
            }
            return query.Any();
        }

        public void AddDepartment(Department dept)
        {
            _context.Departments.Add(dept);
        }

        public void UpdateDepartment(Department dept)
        {
            _context.Departments.Update(dept);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}