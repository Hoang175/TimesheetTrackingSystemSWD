using TimesheetTrackingSystemSWD.BLL.DTOs.AdUser;
using TimesheetTrackingSystemSWD.BLL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Models;

namespace TimesheetTrackingSystemSWD.BLL.Services
{
    public class AdUserService : IAdUserService
    {
        private readonly IAdUserRepository _repo;

        public AdUserService(IAdUserRepository repo)
        {
            _repo = repo;
        }

        public (List<User> Data, int TotalPages) GetPagedUsers(string? keyword, int? roleId, string? status, int page, int pageSize)
        {
            var query = _repo.GetAllUsers();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u =>
                    u.Username.ToLower().Contains(keyword) ||
                    u.Email.ToLower().Contains(keyword) ||
                    u.FullName.ToLower().Contains(keyword));
            }

            if (roleId.HasValue)
                query = query.Where(u => u.RoleId == roleId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(u => u.Status == status);

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var data = query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (data, totalPages);
        }

        public List<Role> GetRoles()
        {
            return _repo.GetAllRoles();
        }

        public bool ChangeRole(AdUserChangeRoleDTO dto, int currentAdminId, out string message)
        {
            message = "";
            var user = _repo.GetUserById(dto.UserId);

            if (user == null) { message = "Không tìm thấy tài khoản."; return false; }

            // Chặn Admin tự hạ quyền của chính mình
            if (user.UserId == currentAdminId && dto.RoleId != user.RoleId)
            {
                message = "Bạn không thể tự thay đổi quyền của chính mình.";
                return false;
            }

            user.RoleId = dto.RoleId;
            _repo.UpdateUser(user);
            _repo.Save();
            return true;
        }

        public bool ResetPassword(AdUserResetPasswordDTO dto, out string message)
        {
            message = "";
            var user = _repo.GetUserById(dto.UserId);

            if (user == null) { message = "Không tìm thấy tài khoản."; return false; }

            //user.PasswordHash = dto.NewPassword;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            _repo.UpdateUser(user);
            _repo.Save();
            return true;
        }

        public bool ToggleStatus(int userId, int currentAdminId, out string message)
        {
            message = "";
            var user = _repo.GetUserById(userId);

            if (user == null) { message = "Không tìm thấy tài khoản."; return false; }

            // Chặn Admin tự khóa tài khoản của chính mình
            if (user.UserId == currentAdminId)
            {
                message = "Bạn không thể tự khóa tài khoản đang đăng nhập của mình.";
                return false;
            }

            // Lật trạng thái: Nếu đang Active thì thành Locked, ngược lại.
            user.Status = (user.Status == "Active") ? "Locked" : "Active";

            _repo.UpdateUser(user);
            _repo.Save();
            return true;
        }
    }
}