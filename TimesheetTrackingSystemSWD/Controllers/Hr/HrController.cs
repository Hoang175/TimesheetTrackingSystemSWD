using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimesheetTrackingSystemSWD.Controllers.Hr
{
    [Authorize(Roles = "HR")] 
    [Route("TimesheetTrackingSystem/hr")]
    public class HrController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("~/Views/Hr/Dashboard/Index.cshtml");
        }
    }
}