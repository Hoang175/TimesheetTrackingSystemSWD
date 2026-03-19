using System;
using System.Collections.Generic;

namespace TimesheetTrackingSystemSWD.DAL.Models;

public partial class SystemLog
{
    public int LogId { get; set; }

    public int UserId { get; set; }

    public string Action { get; set; } = null!;

    public DateTime? Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
