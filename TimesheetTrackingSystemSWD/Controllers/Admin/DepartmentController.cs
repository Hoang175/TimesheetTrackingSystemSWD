using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimesheetTrackingSystemSWD.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("TimesheetTrackingSystem/admin/deparments")]
    public class DepartmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
