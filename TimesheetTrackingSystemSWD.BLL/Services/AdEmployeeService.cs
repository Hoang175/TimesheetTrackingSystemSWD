using System.Globalization;
using System.Text;
using TimesheetTrackingSystemSWD.BLL.DTOs.AdEmployee;
using TimesheetTrackingSystemSWD.BLL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;


// @HoangDH
namespace TimesheetTrackingSystemSWD.BLL.Services
{
    public class AdEmployeeService : IAdEmployeeService
    {
        private readonly IAdEmployeeRepository _repo;

        public AdEmployeeService(IAdEmployeeRepository repo)
        {
            _repo = repo;
        }

        // ==========================================
        // 1. CÁC HÀM TIỆN ÍCH (HELPERS)
        // ==========================================
        private string GenerateEmployeeCode()
        {
            string yearPrefix = DateTime.Now.ToString("yy");
            string prefix = $"EMP{yearPrefix}";

            string? lastCode = _repo.GetLastEmployeeCode(prefix);
            int nextSequence = 1;

            if (!string.IsNullOrEmpty(lastCode))
            {
                string numberPart = lastCode.Substring(prefix.Length);
                if (int.TryParse(numberPart, out int currentSequence))
                {
                    nextSequence = currentSequence + 1;
                }
            }
            return $"{prefix}{nextSequence.ToString("D4")}";
        }

        private string RemoveSign4VietnameseString(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            string formD = str.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }
            string result = sb.ToString().Normalize(NormalizationForm.FormC);

            return result.Replace('đ', 'd').Replace('Đ', 'D');
        }

        private string GenerateSystemUsername(string fullName, string empCode)
        {
            if (string.IsNullOrEmpty(fullName)) return empCode.ToLower();

            string unsignedName = RemoveSign4VietnameseString(fullName).ToLower().Trim();
            var parts = unsignedName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0) return empCode.ToLower();

            string name = parts[parts.Length - 1];
            string initials = "";
            for (int i = 0; i < parts.Length - 1; i++)
            {
                initials += parts[i][0];
            }

            return $"{name}{initials}{empCode.ToLower()}";
        }

        // ==========================================
        // 2. CÁC HÀM NGHIỆP VỤ (BUSINESS LOGIC)
        // ==========================================

        public (List<Employee> Data, int TotalPages) GetPagedEmployees(string? keyword, int? deptId, string? status, int page, int pageSize)
        {
            bool isDeletedView = (status == "Deleted"); 
            var query = _repo.GetAllEmployees(isDeletedView);

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(e =>
                    (e.User != null && e.User.FullName.ToLower().Contains(keyword)) ||
                    e.EmployeeCode.ToLower().Contains(keyword));
            }

            if (deptId.HasValue)
                query = query.Where(e => e.DepartmentId == deptId.Value);

            if (!isDeletedView && !string.IsNullOrEmpty(status))
                query = query.Where(e => e.EmploymentStatus == status);

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var data = query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (data, totalPages);
        }

        public bool CreateEmployee(AdEmployeeCreateDTO dto, out string message)
        {
            message = "";
            try
            {
                if (_repo.CheckEmailExists(dto.Email))
                {
                    message = "Email này đã được sử dụng trong hệ thống.";
                    return false;
                }

                string empCode = GenerateEmployeeCode();

                var emp = new Employee
                {
                    EmployeeCode = empCode,
                    DepartmentId = dto.DepartmentId,
                    Position = dto.Position,
                    HireDate = dto.HireDate,
                    EmploymentStatus = "Active",
                    IsDeleted = false,
                    CreatedAt = DateTime.Now
                };

                int roleId = _repo.GetRoleIdByName("Employee");

                var user = new User
                {
                    Username = GenerateSystemUsername(dto.FullName, empCode),
                    Email = dto.Email,
                    FullName = dto.FullName,
                    PasswordHash = dto.IsCreateAccount ? (string.IsNullOrEmpty(dto.Password) ? "123" : dto.Password) : "NO_LOGIN",
                    RoleId = roleId,
                    Status = dto.IsCreateAccount ? "Active" : "Locked",
                    IsDeleted = false,
                    CreatedAt = DateTime.Now
                };

                emp.User = user;

                _repo.AddEmployee(emp);
                _repo.Save();
                return true;
            }
            catch (Exception ex)
            {
                message = "Lỗi hệ thống: " + (ex.InnerException?.Message ?? ex.Message);
                return false;
            }
        }

        public AdEmployeeUpdateDTO? GetEmployeeForEdit(int id)
        {
            var emp = _repo.GetEmployeeById(id);
            if (emp == null) return null;

            return new AdEmployeeUpdateDTO
            {
                EmployeeId = emp.EmployeeId,
                FullName = emp.User != null ? emp.User.FullName : "N/A",
                Email = emp.User != null ? emp.User.Email : "N/A",
                DepartmentId = emp.DepartmentId,
                Position = emp.Position,
                HireDate = emp.HireDate,
                HasAccount = emp.User != null && emp.User.Status == "Active"
            };
        }

        public bool UpdateEmployee(AdEmployeeUpdateDTO dto, out string message)
        {
            message = "";
            try
            {
                var emp = _repo.GetEmployeeById(dto.EmployeeId);
                if (emp == null) { message = "Không tìm thấy nhân viên."; return false; }

                emp.DepartmentId = dto.DepartmentId;
                emp.Position = dto.Position;
                emp.HireDate = dto.HireDate;

                if (emp.User != null)
                {
                    if (emp.User.Email != dto.Email && _repo.CheckEmailExists(dto.Email))
                    {
                        message = "Email này đã được sử dụng bởi người khác.";
                        return false;
                    }

                    emp.User.FullName = dto.FullName;
                    emp.User.Email = dto.Email;

                    if (!dto.HasAccount && dto.IsCreateAccount)
                    {
                        emp.User.Status = "Active";
                        emp.User.PasswordHash = string.IsNullOrEmpty(dto.Password) ? "123" : dto.Password;
                    }
                }

                _repo.UpdateEmployee(emp);
                _repo.Save();
                return true;
            }
            catch (Exception ex)
            {
                message = "Lỗi hệ thống: " + (ex.InnerException?.Message ?? ex.Message);
                return false;
            }
        }

        public bool InactiveEmployee(int id, out string message)
        {
            message = "";
            var emp = _repo.GetEmployeeById(id);
            if (emp == null) { message = "Không tìm thấy nhân viên."; return false; }

            emp.EmploymentStatus = "Inactive";
            if (emp.User != null) emp.User.Status = "Locked";

            _repo.UpdateEmployee(emp);
            _repo.Save();
            return true;
        }

        public bool ActiveEmployee(int id, out string message)
        {
            message = "";
            var emp = _repo.GetEmployeeById(id);
            if (emp == null) { message = "Không tìm thấy nhân viên."; return false; }

            emp.EmploymentStatus = "Active";
            if (emp.User != null) emp.User.Status = "Active";

            _repo.UpdateEmployee(emp);
            _repo.Save();
            return true;
        }

        public bool DeleteEmployee(int id, out string message)
        {
            message = "";
            var emp = _repo.GetEmployeeById(id);
            if (emp == null) { message = "Không tìm thấy nhân viên."; return false; }

            emp.IsDeleted = true;
            if (emp.User != null) emp.User.IsDeleted = true;

            _repo.UpdateEmployee(emp);
            _repo.Save();
            return true;
        }

        public bool RestoreEmployee(int id, out string message)
        {
            message = "";
            var emp = _repo.GetEmployeeById(id, true); // true = tìm người đã xóa
            if (emp == null) { message = "Không tìm thấy nhân viên."; return false; }

            emp.IsDeleted = false;
            if (emp.User != null) emp.User.IsDeleted = false;

            _repo.UpdateEmployee(emp);
            _repo.Save();
            return true;
        }
    }
}