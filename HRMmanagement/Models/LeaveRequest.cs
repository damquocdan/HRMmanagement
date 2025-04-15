using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMmanagement.Models;

public partial class LeaveRequest : IValidatableObject
{
    public int LeaveId { get; set; }

    [Display(Name = "Mã nhân viên")]
    public int? EmployeeId { get; set; }

    [Display(Name = "Loại nghỉ phép")]
    public string? LeaveType { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu.")]
    [Display(Name = "Ngày bắt đầu")]
    [DataType(DataType.Date)]
    public DateOnly? StartDate { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc.")]
    [Display(Name = "Ngày kết thúc")]
    [DataType(DataType.Date)]
    public DateOnly? EndDate { get; set; }

    [Display(Name = "Lý do")]
    public string? Reason { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Ngày tạo")]
    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Nhân viên")]
    public virtual Employee? Employee { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        var today = DateOnly.FromDateTime(DateTime.Today);

        if (StartDate.HasValue && StartDate.Value < today)
        {
            errors.Add(new ValidationResult("Ngày bắt đầu phải là hôm nay hoặc sau đó.", new[] { nameof(StartDate) }));
        }

        if (EndDate.HasValue && EndDate.Value < today)
        {
            errors.Add(new ValidationResult("Ngày kết thúc phải là hôm nay hoặc sau đó.", new[] { nameof(EndDate) }));
        }

        if (StartDate.HasValue && EndDate.HasValue && EndDate < StartDate)
        {
            errors.Add(new ValidationResult("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.", new[] { nameof(EndDate) }));
        }

        return errors;
    }
}