using System;
using System.Collections.Generic;

namespace HRMmanagement.Models;

public partial class Position
{
    public int PositionId { get; set; }

    public string PositionName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Recruitment> Recruitments { get; set; } = new List<Recruitment>();
}
