using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimesheetTrackingSystemSWD.BLL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.Controllers.Hr
{
    [Authorize(Roles = "HR")]
    [Route("TimesheetTrackingSystem/hr")]
    public class HrController : Controller
    {
        private readonly ITimesheetService _timesheetService;
        private readonly ISystemLogService _logService;

        public HrController(
            ITimesheetService timesheetService,
            ISystemLogService logService)
        {
            _timesheetService = timesheetService;
            _logService = logService;
        }

        // 1. DASHBOARD
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewBag.PendingCount = await _timesheetService.CountByStatusAsync("Pending");
            ViewBag.ApprovedCount = await _timesheetService.CountByStatusAsync("Approved");
            ViewBag.RejectedCount = await _timesheetService.CountByStatusAsync("Rejected");

            return View("~/Views/Hr/Dashboard/Index.cshtml");
        }

        // 3. DANH SÁCH TIMESHEET (ALL + FILTER)
        [HttpGet("timesheets")]
        public async Task<IActionResult> TimesheetList(string? status)
        {
            IEnumerable<Timesheet> data;

            if (string.IsNullOrEmpty(status))
            {
                data = await _timesheetService.GetAllTimesheetsAsync();
            }
            else
            {
                data = await _timesheetService.GetTimesheetsByStatusAsync(status);
            }

            ViewBag.CurrentStatus = status;

            return View("~/Views/Hr/Timesheet/TimesheetList.cshtml", data);
        }

        // 4. APPROVE
        [HttpGet("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            int? hrId = GetCurrentUserId();

            if (hrId == null)
                return Unauthorized();

            await _timesheetService.ApproveAsync(id, hrId.Value);

            return RedirectToAction("TimesheetList", new { status = "Pending" });
        }

        // 5. REJECT
        [HttpGet("reject/{id}")]
        public async Task<IActionResult> Reject(int id)
        {
            int? hrId = GetCurrentUserId();

            if (hrId == null)
                return Unauthorized();

            await _timesheetService.RejectAsync(id, hrId.Value);

            return RedirectToAction("TimesheetList", new { status = "Pending" });
        }

        // 6. Lấy user hiện tại
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return null;

            return int.Parse(userIdClaim.Value);
        }

        // 7. SYSTEM LOGS (Timesheet only)
        [HttpGet("logs")]
        public async Task<IActionResult> Logs()
        {
            var logs = await _logService.GetTimesheetLogsAsync();
            return View("~/Views/Hr/SystemLog/Index.cshtml", logs);
        }
    }
}