using System;
using System.Collections.Generic;

namespace HRMmanagement.Models;

public partial class Training
{
    public int TrainingId { get; set; }

    public int? EmployeeId { get; set; }

    public string? TrainingName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Result { get; set; }

    public string? Evaluation { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee? Employee { get; set; }
}
