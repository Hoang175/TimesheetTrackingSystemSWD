using System;
using System.Collections.Generic;

namespace TimesheetTrackingSystemSWD.DAL.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int UserId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public string? Position { get; set; }

    public DateOnly? HireDate { get; set; }

    public string? EmploymentStatus { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

    public virtual Department? Department { get; set; }

    public virtual ICollection<Timesheet> Timesheets { get; set; } = new List<Timesheet>();

    public virtual User User { get; set; } = null!;
}
