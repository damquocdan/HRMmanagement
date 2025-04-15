using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMmanagement.Models;

public partial class Training
{
    public int TrainingId { get; set; }

    [Display(Name = "Mã nhân viên")]
    public int? EmployeeId { get; set; }

    [Required(ErrorMessage = "Tên khóa đào tạo là bắt buộc.")]
    [Display(Name = "Tên khóa đào tạo")]
    public string? TrainingName { get; set; }

    [Display(Name = "Ngày bắt đầu")]
    [DataType(DataType.Date)]
    public DateOnly? StartDate { get; set; }

    [Display(Name = "Ngày kết thúc")]
    [DataType(DataType.Date)]
    public DateOnly? EndDate { get; set; }

    [Display(Name = "Kết quả")]
    public string? Result { get; set; }

    [Display(Name = "Đánh giá")]
    public string? Evaluation { get; set; }

    [Display(Name = "Ngày tạo")]
    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Nhân viên")]
    public virtual Employee? Employee { get; set; }
}