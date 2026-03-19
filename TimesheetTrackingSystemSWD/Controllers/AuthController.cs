using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimesheetTrackingSystemSWD.BLL.DTOs;
using TimesheetTrackingSystemSWD.BLL.Helpers;
using TimesheetTrackingSystemSWD.BLL.Interfaces;


// @HoangDH
namespace TimesheetTrackingSystemSWD.Controllers
{
    [AllowAnonymous]
    [Route("TimesheetTrackingSystem")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                if (User.IsInRole("Admin")) return RedirectToAction("Index", "Admin");
                if (User.IsInRole("HR")) return RedirectToAction("Index", "Hr");
                if (User.IsInRole("Employee")) return RedirectToAction("Index", "Employee");
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid) return View(model);

            var userSession = await _authService.LoginAsync(model);

            if (userSession == null)
            {
                ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác.";
                return View(model);
            }

            HttpContext.Session.SetObject("USER_SESSION", userSession);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userSession.UserId.ToString()),
                new Claim(ClaimTypes.Name, userSession.Username),
                new Claim("FullName", userSession.FullName),
                new Claim(ClaimTypes.Role, userSession.RoleName)
            };

            if (userSession.EmployeeId.HasValue)
            {
                claims.Add(new Claim("EmployeeId", userSession.EmployeeId.Value.ToString()));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return userSession.RoleName switch
            {
                "Admin" => RedirectToAction("Index", "Admin"), 
                "HR" => RedirectToAction("Index", "HR"),
                "Employee" => RedirectToAction("Index", "Employee"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear(); 
            return RedirectToAction("Login");
        }

        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}