using System;
using System.Collections.Generic;

namespace HRMmanagement.Models;

public partial class PerformanceEvaluation
{
    public int EvaluationId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly? EvaluationDate { get; set; }

    public int? EvaluatorId { get; set; }

    public decimal? SelfScore { get; set; }

    public decimal? ManagerScore { get; set; }

    public decimal? PeerScore { get; set; }

    public string? Comments { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Employee? Evaluator { get; set; }
}
