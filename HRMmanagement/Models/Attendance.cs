using System;
using System.Collections.Generic;

namespace HRMmanagement.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly? AttendanceDate { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public decimal? OvertimeHours { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee? Employee { get; set; }
}
