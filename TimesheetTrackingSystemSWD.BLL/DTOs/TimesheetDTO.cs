using System;
using System.Collections.Generic;

namespace TimesheetTrackingSystemSWD.BLL.DTOs
{
    public class AttendanceRecordDTO
    {
        public int AttendanceId { get; set; }
        public DateOnly WorkDate { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public decimal? DailyHours { get; set; }
        public bool IsComplete { get; set; }
    }

    public class TimesheetSubmissionDTO
    {
        public int EmployeeId { get; set; }
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
        public string PeriodType { get; set; } // "Weekly" or "Monthly"
        public decimal TotalWorkingHours { get; set; }
        public List<AttendanceRecordDTO> AttendanceRecords { get; set; }
        public bool CanSubmit { get; set; }
        public string? ValidationMessage { get; set; }
    }
}
