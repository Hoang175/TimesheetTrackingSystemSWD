//using System.ComponentModel.DataAnnotations;

//namespace TimesheetTrackingSystemSWD.BLL.DTOs.AdEmployee
//{
//    public class AdEmployeeUpdateDTO
//    {
//        public int EmployeeId { get; set; }

//        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
//        public string FullName { get; set; } = null!;

//        [Required(ErrorMessage = "Vui lòng nhập Email")]
//        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
//        public string Email { get; set; } = null!;

//        public int? DepartmentId { get; set; }

//        public string? Position { get; set; }

//        public DateOnly? HireDate { get; set; }
//    }
//}

using System.ComponentModel.DataAnnotations;

namespace TimesheetTrackingSystemSWD.BLL.DTOs.AdEmployee
{
    public class AdEmployeeUpdateDTO
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = null!;

        public int? DepartmentId { get; set; }
        public string? Position { get; set; }
        public DateOnly? HireDate { get; set; }


        // --- CÁC TRƯỜNG BỔ SUNG CHO CHỨC NĂNG CẤP TÀI KHOẢN ---
        public bool HasAccount { get; set; } // Kiểm tra xem đã có tài khoản Active chưa
        public bool IsCreateAccount { get; set; } // Checkbox bật/tắt cấp tài khoản
        public string? Password { get; set; } // Mật khẩu mới
    }
}