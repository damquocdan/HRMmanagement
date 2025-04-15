using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMmanagement.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    [Required(ErrorMessage = "Tên phòng ban là bắt buộc.")]
    [Display(Name = "Tên phòng ban")]
    public string DepartmentName { get; set; } = null!;

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}