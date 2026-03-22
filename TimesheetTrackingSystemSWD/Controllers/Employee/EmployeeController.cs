using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimesheetTrackingSystemSWD.BLL.Interfaces;

namespace TimesheetTrackingSystemSWD.Controllers.Employee
{
    [Authorize(Roles = "Employee")]
    [Route("TimesheetTrackingSystem/employee")]
    public class EmployeeController : Controller
    {
        private readonly ITimesheetService _timesheetService;
        private readonly IAttendanceService _attendanceService;

        public EmployeeController(ITimesheetService timesheetService, IAttendanceService attendanceService)
        {
            _timesheetService = timesheetService;
            _attendanceService = attendanceService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("~/Views/Employee/Dashboard/Index.cshtml");
        }

        [HttpGet("attendance")]
        public async Task<IActionResult> Attendance()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var todayRecord = await _attendanceService.GetTodayAttendanceAsync(userId);
            
            var today = DateTime.Today;
            var monthlyRecords = await _attendanceService.GetMonthlyAttendanceAsync(userId, today.Year, today.Month);
            ViewData["MonthlyRecords"] = monthlyRecords;

            return View("~/Views/Employee/Attendance/Index.cshtml", todayRecord);
        }

        [HttpPost("attendance/checkin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            
            try
            {
                var result = await _attendanceService.CheckInAsync(userId, ipAddress);
                if (result) TempData["SuccessMessage"] = "Check-in thành công!";
                else TempData["ErrorMessage"] = "Bạn đã check-in hôm nay rồi.";
            }
            catch (Exception ex) when (ex.Message == "IP_NOT_ALLOWED")
            {
                TempData["ErrorMessage"] = "Vui lòng kết nối Wifi công ty để thực hiện chấm công.";
            }
            
            return RedirectToAction("Attendance");
        }

        [HttpPost("attendance/checkout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

            try
            {
                var result = await _attendanceService.CheckOutAsync(userId, ipAddress);
                if (result) TempData["SuccessMessage"] = "Check-out thành công!";
                else TempData["ErrorMessage"] = "Không thể check-out (Có thể bạn chưa check-in hoặc đã check-out).";
            }
            catch (Exception ex) when (ex.Message == "IP_NOT_ALLOWED")
            {
                TempData["ErrorMessage"] = "Vui lòng kết nối Wifi công ty để thực hiện chấm công.";
            }

            return RedirectToAction("Attendance");
        }

        [HttpGet("mytimesheet")]
        public async Task<IActionResult> MyTimesheet()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var data = await _timesheetService.GetCurrentTimesheetSubmissionDataAsync(userId);
            
            if (data == null) return NotFound("Employee profile not found.");

            return View("~/Views/Employee/Timesheet/Index.cshtml", data);
        }

        [HttpPost("mytimesheet/submit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(DateOnly periodStart, DateOnly periodEnd)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _timesheetService.SubmitTimesheetAsync(userId, periodStart, periodEnd);

            if (result)
            {
                TempData["SuccessMessage"] = "Timesheet đã được nộp thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi nộp timesheet. Vui lòng kiểm tra lại.";
            }

            return RedirectToAction("MyTimesheet");
        }
    }
}