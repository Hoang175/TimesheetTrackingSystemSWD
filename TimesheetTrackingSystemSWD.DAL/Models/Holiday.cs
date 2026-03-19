using System;
using System.Collections.Generic;

namespace TimesheetTrackingSystemSWD.DAL.Models;

public partial class Holiday
{
    public int HolidayId { get; set; }

    public DateOnly HolidayDate { get; set; }

    public string HolidayName { get; set; } = null!;

    public bool? IsPaidLeave { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
