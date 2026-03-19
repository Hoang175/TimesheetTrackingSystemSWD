using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimesheetTrackingSystemSWD.BLL.DTOs;

// @HoangDH
namespace TimesheetTrackingSystemSWD.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<UserSessionDTO?> LoginAsync(LoginDTO loginDto);
    }
}
