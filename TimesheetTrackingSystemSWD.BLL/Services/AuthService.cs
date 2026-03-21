using TimesheetTrackingSystemSWD.BLL.DTOs;
using TimesheetTrackingSystemSWD.BLL.Interfaces;
using TimesheetTrackingSystemSWD.DAL.Interfaces;

// @HoangDH
namespace TimesheetTrackingSystemSWD.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserSessionDTO?> LoginAsync(LoginDTO loginDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);

            if (user == null || user.Status != "Active") return null;

            //if (user.PasswordHash != loginDto.Password) return null;
            // KIỂM TRA MẬT KHẨU AN TOÀN
            bool isPasswordValid = false;

            // Dấu hiệu "$2" ở đầu chuỗi là đặc trưng của thuật toán BCrypt
            if (user.PasswordHash.StartsWith("$2"))
            {
                // Giải mã và so sánh
                isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            }
            else
            {
                // Fallback: Dành cho các tài khoản cũ trong DB lúc chưa cài BCrypt (mật khẩu "123")
                isPasswordValid = (user.PasswordHash == loginDto.Password);
            }

            if (!isPasswordValid) return null;

            return new UserSessionDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                RoleName = user.Role.RoleName, 
                EmployeeId = user.Employee?.EmployeeId
            };
        }
    }
}