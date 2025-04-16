using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMmanagement.Models;

public partial class Payroll
{
    public int PayrollId { get; set; }

    [Display(Name = "Mã nhân viên")]
    public int? EmployeeId { get; set; }

    [Display(Name = "Tháng")]
    [Range(1, 12, ErrorMessage = "Tháng phải nằm trong khoảng từ 1 đến 12.")]
    public int? Month { get; set; }

    [Display(Name = "Năm")]
    [Range(2000, 2100, ErrorMessage = "Năm phải nằm trong khoảng từ 2000 đến 2100.")]
    public int? Year { get; set; }

    [Display(Name = "Lương công")]
    [Range(0, double.MaxValue, ErrorMessage = "Lương cơ bản phải là một số không âm.")]
    public decimal? BaseSalary { get; set; }

    [Display(Name = "Phụ cấp")]
    [Range(0, double.MaxValue, ErrorMessage = "Phụ cấp phải là một số không âm.")]
    public decimal? Allowance { get; set; }

    [Display(Name = "Thưởng")]
    [Range(0, double.MaxValue, ErrorMessage = "Thưởng phải là một số không âm.")]
    public decimal? Bonus { get; set; }

    [Display(Name = "Thuế")]
    public decimal? Tax { get; set; }

    [Display(Name = "Bảo hiểm")]
    [Range(0, double.MaxValue, ErrorMessage = "Bảo hiểm phải là một số không âm.")]
    public decimal? Insurance { get; set; }

    [Display(Name = "Lương thực nhận")]
    public decimal? NetSalary { get; set; }

    [Display(Name = "Ngày tạo")]
    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; }
    public string? Status { get; set; }
    [Display(Name = "Nhân viên")]
    public virtual Employee? Employee { get; set; }
}