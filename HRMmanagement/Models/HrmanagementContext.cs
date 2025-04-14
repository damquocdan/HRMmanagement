using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HRMmanagement.Models;

public partial class HrmanagementContext : DbContext
{
    public HrmanagementContext()
    {
    }

    public HrmanagementContext(DbContextOptions<HrmanagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<Payroll> Payrolls { get; set; }

    public virtual DbSet<PerformanceEvaluation> PerformanceEvaluations { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Recruitment> Recruitments { get; set; }

    public virtual DbSet<Training> Training { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DAMQUOCDAN;Database=HRManagement;uid=sa;pwd=1234;MultipleActiveResultSets=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admins__719FE4E85AA2042A");

            entity.HasIndex(e => e.Username, "UQ__Admins__536C85E4AAE88661").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__8B69263CA106D461");

            entity.ToTable("Attendance");

            entity.Property(e => e.AttendanceId).HasColumnName("AttendanceID");
            entity.Property(e => e.CheckInTime).HasColumnType("datetime");
            entity.Property(e => e.CheckOutTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.OvertimeHours).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Employee).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Attendanc__Emplo__44FF419A");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BCDAFF7DA8B");

            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1EF08343E");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.BaseSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.QrcodeData)
                .HasMaxLength(200)
                .HasColumnName("QRCodeData");

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Employees__Depar__403A8C7D");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK__Employees__Posit__412EB0B6");
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.LeaveId).HasName("PK__LeaveReq__796DB9794CA5E9D4");

            entity.Property(e => e.LeaveId).HasColumnName("LeaveID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.LeaveType).HasMaxLength(50);
            entity.Property(e => e.Reason).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Employee).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__LeaveRequ__Emplo__48CFD27E");
        });

        modelBuilder.Entity<Payroll>(entity =>
        {
            entity.HasKey(e => e.PayrollId).HasName("PK__Payroll__99DFC692C92B4126");

            entity.ToTable("Payroll");

            entity.Property(e => e.PayrollId).HasColumnName("PayrollID");
            entity.Property(e => e.Allowance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BaseSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Bonus).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Insurance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NetSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Tax).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Employee).WithMany(p => p.Payrolls)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Payroll__Employe__4CA06362");
        });

        modelBuilder.Entity<PerformanceEvaluation>(entity =>
        {
            entity.HasKey(e => e.EvaluationId).HasName("PK__Performa__36AE68D33CC77F4E");

            entity.Property(e => e.EvaluationId).HasColumnName("EvaluationID");
            entity.Property(e => e.Comments).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.EvaluatorId).HasColumnName("EvaluatorID");
            entity.Property(e => e.ManagerScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PeerScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.SelfScore).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Employee).WithMany(p => p.PerformanceEvaluationEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Performan__Emplo__5812160E");

            entity.HasOne(d => d.Evaluator).WithMany(p => p.PerformanceEvaluationEvaluators)
                .HasForeignKey(d => d.EvaluatorId)
                .HasConstraintName("FK__Performan__Evalu__59063A47");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__60BB9A59A8C36FE2");

            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.PositionName).HasMaxLength(100);
        });

        modelBuilder.Entity<Recruitment>(entity =>
        {
            entity.HasKey(e => e.CandidateId).HasName("PK__Recruitm__DF539BFC4AF88D6F");

            entity.ToTable("Recruitment");

            entity.Property(e => e.CandidateId).HasColumnName("CandidateID");
            entity.Property(e => e.AppliedPositionId).HasColumnName("AppliedPositionID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Cvpath)
                .HasMaxLength(200)
                .HasColumnName("CVPath");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.InterviewDate).HasColumnType("datetime");
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.AppliedPosition).WithMany(p => p.Recruitments)
                .HasForeignKey(d => d.AppliedPositionId)
                .HasConstraintName("FK__Recruitme__Appli__5070F446");
        });

        modelBuilder.Entity<Training>(entity =>
        {
            entity.HasKey(e => e.TrainingId).HasName("PK__Training__E8D71DE2EAA0B6AF");

            entity.Property(e => e.TrainingId).HasColumnName("TrainingID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Evaluation).HasMaxLength(200);
            entity.Property(e => e.Result).HasMaxLength(50);
            entity.Property(e => e.TrainingName).HasMaxLength(100);

            entity.HasOne(d => d.Employee).WithMany(p => p.Training)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Training__Employ__5441852A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
