using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMmanagement.Models;

public partial class Position
{
    public int PositionId { get; set; }

    [Required(ErrorMessage = "Tên chức vụ là bắt buộc.")]
    [Display(Name = "Tên chức vụ")]
    public string PositionName { get; set; } = null!;

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Recruitment> Recruitments { get; set; } = new List<Recruitment>();
}