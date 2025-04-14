using System;
using System.Collections.Generic;

namespace HRMmanagement.Models;

public partial class Payroll
{
    public int PayrollId { get; set; }

    public int? EmployeeId { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public decimal? BaseSalary { get; set; }

    public decimal? Allowance { get; set; }

    public decimal? Bonus { get; set; }

    public decimal? Tax { get; set; }

    public decimal? Insurance { get; set; }

    public decimal? NetSalary { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee? Employee { get; set; }
}
