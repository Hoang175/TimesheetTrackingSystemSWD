using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TimesheetTrackingSystemSWD.BLL.DTOs.AdEmployee;
using TimesheetTrackingSystemSWD.BLL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;


// @HoangDH
namespace TimesheetTrackingSystemSWD.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("TimesheetTrackingSystem/admin/employees")]
    public class AdEmployeeController : Controller
    {
        private readonly IAdEmployeeService _service;
        private readonly TimesheetTrackingSystemSwdContext _context;

        public AdEmployeeController(IAdEmployeeService service, TimesheetTrackingSystemSwdContext context)
        {
            _service = service;
            _context = context;
        }

        private void LoadViewBags(int? selectedDeptId = null)
        {
            var departments = _context.Departments.Where(d => d.IsDeleted == false).ToList();
            ViewBag.Departments = new SelectList(departments, "DepartmentId", "DepartmentName", selectedDeptId);
        }

        [HttpGet("")]
        public IActionResult Index(string? keyword, int? deptId, string? status, int page = 1)
        {
            int pageSize = 10;
            var result = _service.GetPagedEmployees(keyword, deptId, status, page, pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.Keyword = keyword;
            ViewBag.DeptId = deptId;
            ViewBag.Status = status;

            LoadViewBags(deptId);
            return View("~/Views/Admin/Employee/Index.cshtml", result.Data);
        }

        // --- CREATE ---
        [HttpGet("create")]
        public IActionResult Create()
        {
            LoadViewBags();
            return View("~/Views/Admin/Employee/Create.cshtml", new AdEmployeeCreateDTO());
        }

        [HttpPost("create")]
        public IActionResult Create(AdEmployeeCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                LoadViewBags(dto.DepartmentId);
                return View("~/Views/Admin/Employee/Create.cshtml", dto);
            }

            if (_service.CreateEmployee(dto, out string message))
            {
                TempData["Success"] = "Đã thêm nhân viên thành công.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", message);
            LoadViewBags(dto.DepartmentId);
            return View("~/Views/Admin/Employee/Create.cshtml", dto);
        }

        // --- EDIT ---
        [HttpGet("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var dto = _service.GetEmployeeForEdit(id);
            if (dto == null) return NotFound();

            LoadViewBags(dto.DepartmentId);
            return View("~/Views/Admin/Employee/Edit.cshtml", dto);
        }

        [HttpPost("edit/{id}")]
        public IActionResult Edit(int id, AdEmployeeUpdateDTO dto)
        {
            dto.EmployeeId = id;
            if (!ModelState.IsValid)
            {
                LoadViewBags(dto.DepartmentId);
                return View("~/Views/Admin/Employee/Edit.cshtml", dto);
            }

            if (_service.UpdateEmployee(dto, out string message))
            {
                TempData["Success"] = "Cập nhật hồ sơ thành công.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", message);
            LoadViewBags(dto.DepartmentId);
            return View("~/Views/Admin/Employee/Edit.cshtml", dto);
        }

        // --- ACTIONS ---
        [HttpPost("inactive/{id}")]
        public IActionResult Inactive(int id)
        {
            if (_service.InactiveEmployee(id, out string msg))
                TempData["Success"] = "Đã vô hiệu hóa nhân viên.";
            else
                TempData["Error"] = msg;

            return RedirectToAction("Index");
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (_service.DeleteEmployee(id, out string msg))
                TempData["Success"] = "Đã ẩn hồ sơ nhân viên.";
            else
                TempData["Error"] = msg;

            return RedirectToAction("Index");
        }

        [HttpPost("active/{id}")]
        public IActionResult Active(int id)
        {
            if (_service.ActiveEmployee(id, out string msg))
                TempData["Success"] = "Đã kích hoạt lại nhân viên.";
            else
                TempData["Error"] = msg;

            return RedirectToAction("Index");
        }

        [HttpPost("restore/{id}")]
        public IActionResult Restore(int id)
        {
            if (_service.RestoreEmployee(id, out string msg))
                TempData["Success"] = "Đã khôi phục hồ sơ thành công.";
            else
                TempData["Error"] = msg;

            return RedirectToAction("Index"); 
        }

    }
}