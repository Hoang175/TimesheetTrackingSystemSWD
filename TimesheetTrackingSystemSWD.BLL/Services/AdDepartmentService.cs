using TimesheetTrackingSystemSWD.BLL.DTOs.AdDepartment;
using TimesheetTrackingSystemSWD.BLL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.BLL.Services
{
    public class AdDepartmentService : IAdDepartmentService
    {
        private readonly IAdDepartmentRepository _repo;

        public AdDepartmentService(IAdDepartmentRepository repo)
        {
            _repo = repo;
        }

        public List<AdDepartmentDTO> GetAllDepartments(string? keyword, bool isDeletedView = false)
        {
            var query = _repo.GetAllDepartments(isDeletedView); 

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(d => d.DepartmentName.ToLower().Contains(keyword));
            }

            return query
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new AdDepartmentDTO
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    Description = d.Description,
                    EmployeeCount = d.Employees.Count()
                }).ToList();
        }

        public bool RestoreDepartment(int id, out string message)
        {
            message = "";
            var dept = _repo.GetDeletedDepartmentById(id);
            if (dept == null) { message = "Không tìm thấy phòng ban."; return false; }

            if (_repo.CheckNameExists(dept.DepartmentName))
            {
                message = "Không thể khôi phục vì đã có một phòng ban khác tên này đang hoạt động.";
                return false;
            }

            dept.IsDeleted = false; 

            _repo.UpdateDepartment(dept);
            _repo.Save();
            return true;
        }

        public AdDepartmentDTO? GetDepartmentForEdit(int id)
        {
            var dept = _repo.GetDepartmentById(id);
            if (dept == null) return null;

            return new AdDepartmentDTO
            {
                DepartmentId = dept.DepartmentId,
                DepartmentName = dept.DepartmentName,
                Description = dept.Description
            };
        }

        public bool CreateDepartment(AdDepartmentDTO dto, out string message)
        {
            message = "";
            if (_repo.CheckNameExists(dto.DepartmentName.Trim()))
            {
                message = "Tên phòng ban đã tồn tại.";
                return false;
            }

            var dept = new Department
            {
                DepartmentName = dto.DepartmentName.Trim(),
                Description = dto.Description?.Trim(),
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };

            _repo.AddDepartment(dept);
            _repo.Save();
            return true;
        }

        public bool UpdateDepartment(AdDepartmentDTO dto, out string message)
        {
            message = "";
            var dept = _repo.GetDepartmentById(dto.DepartmentId);
            if (dept == null) { message = "Không tìm thấy phòng ban."; return false; }

            if (_repo.CheckNameExists(dto.DepartmentName.Trim(), dto.DepartmentId))
            {
                message = "Tên phòng ban này đã được sử dụng.";
                return false;
            }

            dept.DepartmentName = dto.DepartmentName.Trim();
            dept.Description = dto.Description?.Trim();
            dept.UpdatedAt = DateTime.Now;

            _repo.UpdateDepartment(dept);
            _repo.Save();
            return true;
        }

        public bool DeleteDepartment(int id, out string message)
        {
            message = "";
            var dept = _repo.GetDepartmentById(id);
            if (dept == null) { message = "Không tìm thấy phòng ban."; return false; }

            
            if (dept.Employees.Any())
            {
                message = $"Không thể xóa! Đang có {dept.Employees.Count} nhân viên thuộc phòng ban này.";
                return false;
            }

            dept.IsDeleted = true;
            _repo.UpdateDepartment(dept);
            _repo.Save();
            return true;
        }
    }
}