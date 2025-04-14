using System;
using System.Collections.Generic;

namespace HRMmanagement.Models;

public partial class Recruitment
{
    public int CandidateId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? AppliedPositionId { get; set; }

    public string? Cvpath { get; set; }

    public DateTime? InterviewDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Position? AppliedPosition { get; set; }
}
