using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMmanagement.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = null!;

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [Display(Name = "Giới tính")]
    public string? Gender { get; set; }

    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "Phòng ban")]
    public int? DepartmentId { get; set; }

    [Display(Name = "Chức vụ")]
    public int? PositionId { get; set; }

    [Display(Name = "Lương cơ bản")]
    [Range(0, double.MaxValue, ErrorMessage = "Lương cơ bản phải là một số không âm.")]
    public decimal? BaseSalary { get; set; }

    [Display(Name = "Ngày bắt đầu hợp đồng")]
    [DataType(DataType.Date)]
    public DateOnly? ContractStartDate { get; set; }

    [Display(Name = "Ngày kết thúc hợp đồng")]
    [DataType(DataType.Date)]
    public DateOnly? ContractEndDate { get; set; }

    [Display(Name = "Ngày tạo")]
    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Dữ liệu QR Code")]
    public string? QrcodeData { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    [Display(Name = "Phòng ban")]
    public virtual Department? Department { get; set; }

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

    public virtual ICollection<PerformanceEvaluation> PerformanceEvaluationEmployees { get; set; } = new List<PerformanceEvaluation>();

    public virtual ICollection<PerformanceEvaluation> PerformanceEvaluationEvaluators { get; set; } = new List<PerformanceEvaluation>();

    [Display(Name = "Chức vụ")]
    public virtual Position? Position { get; set; }

    public virtual ICollection<Training> Training { get; set; } = new List<Training>();
}