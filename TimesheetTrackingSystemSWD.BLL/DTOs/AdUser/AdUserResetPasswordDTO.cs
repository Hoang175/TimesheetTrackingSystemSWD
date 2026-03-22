using System.ComponentModel.DataAnnotations;

namespace TimesheetTrackingSystemSWD.BLL.DTOs.AdUser
{
    public class AdUserResetPasswordDTO
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [MinLength(3, ErrorMessage = "Mật khẩu phải từ 3 ký tự trở lên.")]
        public string NewPassword { get; set; } = null!;
    }
}