using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TimesheetTrackingSystemSWD.BLL.Interfaces;
using TimesheetTrackingSystemSWD.BLL.Services;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;
using TimesheetTrackingSystemSWD.DAL.Repositories;

namespace TimesheetTrackingSystemSWD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // DB Context
            builder.Services.AddDbContext<TimesheetTrackingSystemSwdContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

            // DI
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            //builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IAdEmployeeRepository, AdEmployeeRepository>();
            builder.Services.AddScoped<IAdEmployeeService, AdEmployeeService>();
            builder.Services.AddScoped<IAdUserRepository, AdUserRepository>();
            builder.Services.AddScoped<IAdUserService, AdUserService>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/TimesheetTrackingSystem/login";
                    options.AccessDeniedPath = "/TimesheetTrackingSystem/access-denied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(8);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}