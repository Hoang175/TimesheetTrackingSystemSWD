using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// @HoangDH
namespace TimesheetTrackingSystemSWD.BLL.DTOs
{
    public class UserSessionDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public int? EmployeeId { get; set; } 
    }
}
