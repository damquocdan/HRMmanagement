using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMmanagement.Models;

public partial class PerformanceEvaluation
{
    public int EvaluationId { get; set; }

    [Display(Name = "Mã nhân viên được đánh giá")]
    public int? EmployeeId { get; set; }

    [Display(Name = "Ngày đánh giá")]
    [DataType(DataType.Date)]
    public DateOnly? EvaluationDate { get; set; }

    [Display(Name = "Mã người đánh giá")]
    public int? EvaluatorId { get; set; }

    [Display(Name = "Điểm tự đánh giá")]
    [Range(0, 10, ErrorMessage = "Điểm tự đánh giá phải nằm trong khoảng từ 0 đến 10.")]
    public decimal? SelfScore { get; set; }

    [Display(Name = "Điểm quản lý đánh giá")]
    [Range(0, 10, ErrorMessage = "Điểm quản lý đánh giá phải nằm trong khoảng từ 0 đến 10.")]
    public decimal? ManagerScore { get; set; }

    [Display(Name = "Điểm đồng nghiệp đánh giá")]
    [Range(0, 10, ErrorMessage = "Điểm đồng nghiệp đánh giá phải nằm trong khoảng từ 0 đến 10.")]
    public decimal? PeerScore { get; set; }

    [Display(Name = "Ghi chú")]
    public string? Comments { get; set; }

    [Display(Name = "Ngày tạo")]
    [DataType(DataType.DateTime)]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Nhân viên được đánh giá")]
    public virtual Employee? Employee { get; set; }

    [Display(Name = "Người đánh giá")]
    public virtual Employee? Evaluator { get; set; }
}