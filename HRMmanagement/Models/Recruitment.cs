using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMmanagement.Models;

public partial class Recruitment
{
    public int CandidateId { get; set; }

    [Required(ErrorMessage = "Họ và tên ứng viên là bắt buộc.")]
    [Display(Name = "Họ và tên")]
    public string? FullName { get; set; }

    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn vị trí ứng tuyển.")]
    [Display(Name = "Vị trí ứng tuyển")]
    public int? AppliedPositionId { get; set; }

    [Display(Name = "Đường dẫn CV")]
    public string? Cvpath { get; set; }

    [Display(Name = "Ngày phỏng vấn")]
    [DataType(DataType.DateTime)]
    public DateTime? InterviewDate { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Ngày tạo")]
    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Vị trí ứng tuyển")]
    public virtual Position? AppliedPosition { get; set; }
}