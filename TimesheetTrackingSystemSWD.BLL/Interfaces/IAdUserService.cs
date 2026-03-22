using TimesheetTrackingSystemSWD.BLL.DTOs.AdUser;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.BLL.Interfaces
{
    public interface IAdUserService
    {
        (List<User> Data, int TotalPages) GetPagedUsers(string? keyword, int? roleId, string? status, int page, int pageSize);
        List<Role> GetRoles();
        bool ChangeRole(AdUserChangeRoleDTO dto, int currentAdminId, out string message);
        bool ResetPassword(AdUserResetPasswordDTO dto, out string message);
        bool ToggleStatus(int userId, int currentAdminId, out string message);
    }
}