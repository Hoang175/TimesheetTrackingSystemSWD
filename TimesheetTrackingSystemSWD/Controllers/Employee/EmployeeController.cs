using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimesheetTrackingSystemSWD.Controllers.Employee
{
    [Authorize(Roles = "Employee")] // Chỉ nhân viên mới được vào
    [Route("TimesheetTrackingSystem/employee")]
    public class EmployeeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("~/Views/Employee/Dashboard/Index.cshtml");
        }
    }
}