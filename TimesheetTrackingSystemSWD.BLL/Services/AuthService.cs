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

            if (user.PasswordHash != loginDto.Password) return null;

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