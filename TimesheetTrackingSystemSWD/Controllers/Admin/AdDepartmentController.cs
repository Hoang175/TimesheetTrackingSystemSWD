using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimesheetTrackingSystemSWD.BLL.DTOs.AdDepartment;
using TimesheetTrackingSystemSWD.BLL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("TimesheetTrackingSystem/admin/departments")]
    public class AdDepartmentController : Controller
    {
        private readonly IAdDepartmentService _service;
        private readonly TimesheetTrackingSystemSwdContext _context;

        public AdDepartmentController(IAdDepartmentService service, TimesheetTrackingSystemSwdContext context)
        {
            _service = service;
            _context = context;
        }


        [HttpGet("")]
        public IActionResult Index(string? keyword, string? status, string? employeeKeyword, int? assignDepartmentId)
        {
            bool isDeletedView = (status == "Deleted");
            var data = _service.GetAllDepartments(keyword, isDeletedView);

            var query = _context.Employees
                .Include(e => e.User)
                .Include(e => e.Department)
                .Where(e => e.IsDeleted == false
                    && e.User != null
                    && e.User.IsDeleted == false
                    && e.EmploymentStatus == "Active");

            if (!string.IsNullOrWhiteSpace(employeeKeyword))
            {
                var kw = employeeKeyword.Trim();
                query = query.Where(e =>
                    e.User.FullName.Contains(kw)
                    || e.EmployeeCode.Contains(kw)
                    || e.User.Email.Contains(kw));
            }

            var assignableEmployees = query
                .OrderBy(e => e.User.FullName)
                .Take(50)
                .Select(e => new
                {
                    Id = e.EmployeeId,
                    DisplayText = $"{e.User.FullName} ({e.EmployeeCode}) - {(e.Department != null ? e.Department.DepartmentName : "Chua phan bo")}",
                    IsSameDepartment = assignDepartmentId.HasValue && e.DepartmentId == assignDepartmentId.Value
                })
                .ToList();

            ViewBag.Keyword = keyword;
            ViewBag.Status = status;
            ViewBag.AssignEmployeeKeyword = employeeKeyword;
            ViewBag.AssignDepartmentId = assignDepartmentId;
            ViewBag.OpenAssignModal = assignDepartmentId.HasValue;
            ViewBag.AssignableEmployees = assignableEmployees;
            return View("~/Views/Admin/Department/Index.cshtml", data);
        }


        [HttpPost("create")]
        public IActionResult Create(AdDepartmentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("Index");
            }

            if (_service.CreateDepartment(dto, out string msg))
                TempData["Success"] = "Đã thêm phòng ban mới.";
            else
                TempData["Error"] = msg;

            return RedirectToAction("Index");
        }

        [HttpPost("edit/{id}")]
        public IActionResult Edit(int id, AdDepartmentDTO dto)
        {
            dto.DepartmentId = id;
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("Index");
            }

            if (_service.UpdateDepartment(dto, out string msg))
                TempData["Success"] = "Cập nhật phòng ban thành công.";
            else
                TempData["Error"] = msg;

            return RedirectToAction("Index");
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (_service.DeleteDepartment(id, out string msg))
                TempData["Success"] = "Đã xóa phòng ban.";
            else
                TempData["Error"] = msg;

            return RedirectToAction("Index");
        }

        [HttpPost("restore/{id}")]
        public IActionResult Restore(int id)
        {
            if (_service.RestoreDepartment(id, out string msg))
                TempData["Success"] = "Đã khôi phục phòng ban.";
            else
                TempData["Error"] = msg;

            return RedirectToAction("Index", new { status = "Deleted" });
        }

        [HttpPost("assign-employee")]
        public IActionResult AssignEmployeeToDepartment(int departmentId, int employeeId)
        {
            var department = _context.Departments.FirstOrDefault(d => d.DepartmentId == departmentId && d.IsDeleted == false);
            if (department == null)
            {
                TempData["Error"] = "Phòng ban không hợp lệ hoặc đã bị xóa.";
                return RedirectToAction("Index");
            }

            var employee = _context.Employees
                .Include(e => e.User)
                .FirstOrDefault(e => e.EmployeeId == employeeId && e.IsDeleted == false);

            if (employee == null)
            {
                TempData["Error"] = "Không tìm thấy nhân viên hợp lệ.";
                return RedirectToAction("Index");
            }

            if (employee.EmploymentStatus != "Active")
            {
                TempData["Error"] = "Không thể thêm nhân viên đã nghỉ việc vào phòng ban.";
                return RedirectToAction("Index");
            }

            if (employee.DepartmentId == departmentId)
            {
                TempData["Error"] = "Nhân viên đã thuộc phòng ban này.";
                return RedirectToAction("Index");
            }

            employee.DepartmentId = departmentId;
            employee.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Success"] = "Đã thêm nhân viên vào phòng ban thành công.";
            return RedirectToAction("Index");
        }
    }
}