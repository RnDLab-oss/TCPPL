using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Model;

public partial class EmpTaskMsDbContext : DbContext
{
    public EmpTaskMsDbContext()
    {
    }

    public EmpTaskMsDbContext(DbContextOptions<EmpTaskMsDbContext> options)
        : base(options)
    {
    }

    //public virtual DbSet<DepartmentMaster> DepartmentMasters { get; set; }

    public virtual DbSet<EmployeeMaster> EmployeeMasters { get; set; }

    public virtual DbSet<TaskManagement> TaskManagements { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=VIshal2000\\SQLEXPRESS;Database=Emp_TaskMS_DB;Password=123456;User Id=sa;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<DepartmentMaster>(entity =>
        //{
        //    entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BEDF503D9AD");

        //    entity.ToTable("DepartmentMaster");

        //    entity.HasIndex(e => e.DepartmentName, "UQ__Departme__D949CC34546E3997").IsUnique();

        //    entity.Property(e => e.CreatedDate)
        //        .HasDefaultValueSql("(getdate())")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.DepartmentName).HasMaxLength(100);
        //    entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        //});

        modelBuilder.Entity<EmployeeMaster>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11BD907745");

            entity.ToTable("EmployeeMaster");

            entity.Property(e => e.ContactNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EmployeeName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(15);
            entity.Property(e => e.Role).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TaskManagement>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__TaskMana__7C6949B187E16ADC");

            entity.ToTable("TaskManagement");

            entity.Property(e => e.AdminRemarks).HasMaxLength(500);
            entity.Property(e => e.AdminStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending Review");
            entity.Property(e => e.AssignedBy)
                .HasMaxLength(20)
                .HasDefaultValue("Admin");
            entity.Property(e => e.AssignedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CompletionDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Deadline).HasColumnType("datetime");
            entity.Property(e => e.EmployeeRemarks).HasMaxLength(500);
            entity.Property(e => e.EmployeeStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TaskTitle).HasMaxLength(200);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.AssignedEmployee).WithMany(p => p.TaskManagements)
                .HasForeignKey(d => d.AssignedEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_AssignedEmployee");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
