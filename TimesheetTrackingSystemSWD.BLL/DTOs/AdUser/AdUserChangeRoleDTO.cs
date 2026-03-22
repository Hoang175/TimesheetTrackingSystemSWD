using System.ComponentModel.DataAnnotations;

namespace TimesheetTrackingSystemSWD.BLL.DTOs.AdUser
{
    public class AdUserChangeRoleDTO
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quyền truy cập.")]
        public int RoleId { get; set; }
    }
}