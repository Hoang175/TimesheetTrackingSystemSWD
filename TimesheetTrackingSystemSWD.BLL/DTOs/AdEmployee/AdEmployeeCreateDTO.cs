using System.ComponentModel.DataAnnotations;

namespace TimesheetTrackingSystemSWD.BLL.DTOs.AdEmployee
{
    public class AdEmployeeCreateDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = null!;

        public int? DepartmentId { get; set; }
        public string? Position { get; set; } // Giờ là string
        public DateOnly? HireDate { get; set; }

        public bool IsCreateAccount { get; set; }
        public string? Password { get; set; }
    }
}