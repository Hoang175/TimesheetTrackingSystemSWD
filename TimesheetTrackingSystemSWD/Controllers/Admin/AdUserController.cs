using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TimesheetTrackingSystemSWD.BLL.DTOs.AdUser;
using TimesheetTrackingSystemSWD.BLL.Interfaces;

namespace TimesheetTrackingSystemSWD.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("TimesheetTrackingSystem/admin/users")]
    public class AdUserController : Controller
    {
        private readonly IAdUserService _service;

        public AdUserController(IAdUserService service)
        {
            _service = service;
        }

        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdStr, out int id) ? id : 0;
        }

        [HttpGet("")]
        public IActionResult Index(string? keyword, int? roleId, string? status, int page = 1)
        {
            int pageSize = 10;
            var result = _service.GetPagedUsers(keyword, roleId, status, page, pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.Keyword = keyword;
            ViewBag.RoleId = roleId;
            ViewBag.Status = status;

            var roles = _service.GetRoles();
            ViewBag.Roles = new SelectList(roles, "RoleId", "RoleName", roleId);

            return View("~/Views/Admin/UserAccount/Index.cshtml", result.Data);
        }

        [HttpPost("change-role")]
        public IActionResult ChangeRole(AdUserChangeRoleDTO dto)
        {
            if (_service.ChangeRole(dto, GetCurrentUserId(), out string msg))
                TempData["Success"] = "Cập nhật phân quyền thành công.";
            else
                TempData["Error"] = msg;

            return RedirectToAction("Index");
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(AdUserResetPasswordDTO dto)
        {
            if (_service.ResetPassword(dto, out string msg))
                TempData["Success"] = "Đã đặt lại mật khẩu thành công.";
            else
                TempData["Error"] = msg;

            return RedirectToAction("Index");
        }

        [HttpPost("toggle-status/{id}")]
        public IActionResult ToggleStatus(int id)
        {
            if (_service.ToggleStatus(id, GetCurrentUserId(), out string msg))
                TempData["Success"] = "Đã cập nhật trạng thái tài khoản.";
            else
                TempData["Error"] = msg;

            return RedirectToAction("Index");
        }
    }
}