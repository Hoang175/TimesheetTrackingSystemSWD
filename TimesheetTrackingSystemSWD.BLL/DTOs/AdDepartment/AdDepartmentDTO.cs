using System.ComponentModel.DataAnnotations;

namespace TimesheetTrackingSystemSWD.BLL.DTOs.AdDepartment
{
    public class AdDepartmentDTO
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Tên phòng ban không được để trống")]
        [StringLength(100, ErrorMessage = "Tên phòng ban không vượt quá 100 ký tự")]
        public string DepartmentName { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Mô tả không vượt quá 255 ký tự")]
        public string? Description { get; set; }

        // Trường dùng riêng cho lúc hiển thị danh sách (không dùng lúc Create/Edit)
        public int EmployeeCount { get; set; }
    }
}