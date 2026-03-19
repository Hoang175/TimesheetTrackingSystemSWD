using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// @HoangDH
namespace TimesheetTrackingSystemSWD.Controllers
{
    [Authorize(Roles = "Admin")] 
    [Route("TimesheetTrackingSystem/admin")] 
    public class AdminController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("~/Views/Admin/Dashboard/Index.cshtml");
        }
    }
}