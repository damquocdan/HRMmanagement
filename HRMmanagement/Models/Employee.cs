using System;
using System.Collections.Generic;

namespace HRMmanagement.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int? DepartmentId { get; set; }

    public int? PositionId { get; set; }

    public decimal? BaseSalary { get; set; }

    public DateOnly? ContractStartDate { get; set; }

    public DateOnly? ContractEndDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? QrcodeData { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Department? Department { get; set; }

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

    public virtual ICollection<PerformanceEvaluation> PerformanceEvaluationEmployees { get; set; } = new List<PerformanceEvaluation>();

    public virtual ICollection<PerformanceEvaluation> PerformanceEvaluationEvaluators { get; set; } = new List<PerformanceEvaluation>();

    public virtual Position? Position { get; set; }

    public virtual ICollection<Training> Training { get; set; } = new List<Training>();
}
