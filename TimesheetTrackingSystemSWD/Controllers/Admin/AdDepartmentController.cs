using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimesheetTrackingSystemSWD.BLL.DTOs.AdDepartment;
using TimesheetTrackingSystemSWD.BLL.Interfaces;

namespace TimesheetTrackingSystemSWD.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("TimesheetTrackingSystem/admin/departments")]
    public class AdDepartmentController : Controller
    {
        private readonly IAdDepartmentService _service;

        public AdDepartmentController(IAdDepartmentService service)
        {
            _service = service;
        }


        [HttpGet("")]
        public IActionResult Index(string? keyword, string? status)
        {
            bool isDeletedView = (status == "Deleted");
            var data = _service.GetAllDepartments(keyword, isDeletedView);

            ViewBag.Keyword = keyword;
            ViewBag.Status = status; 
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
    }
}