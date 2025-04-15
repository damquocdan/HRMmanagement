using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMmanagement.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    [Display(Name = "Mã nhân viên")]
    public int? EmployeeId { get; set; }

    [Display(Name = "Ngày chấm công")]
    [DataType(DataType.Date)]
    public DateOnly? AttendanceDate { get; set; }

    [Display(Name = "Thời gian vào")]
    [DataType(DataType.Time)]
    public DateTime? CheckInTime { get; set; }

    [Display(Name = "Thời gian ra")]
    [DataType(DataType.Time)]
    public DateTime? CheckOutTime { get; set; }

    [Display(Name = "Giờ làm thêm")]
    [Range(0, double.MaxValue, ErrorMessage = "Giờ làm thêm phải là một số không âm.")]
    public decimal? OvertimeHours { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Ngày tạo")]
    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; }

    public virtual Employee? Employee { get; set; }
}