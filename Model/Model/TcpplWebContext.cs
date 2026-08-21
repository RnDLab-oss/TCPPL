using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Model;

public partial class TcpplWebContext : DbContext
{
    public TcpplWebContext()
    {
    }

    public TcpplWebContext(DbContextOptions<TcpplWebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApiLog> ApiLogs { get; set; }
    public virtual DbSet<Branch> Branches { get; set; }
    public virtual DbSet<UserSession> UserSessions { get; set; }
    public virtual DbSet<User> Users { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.10.40;Database=TCPPL_WEB;Password=system;User Id =TCPPL; TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApiLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ApiLog__3214EC07DCE397E0");

            entity.ToTable("ApiLog");

            entity.Property(e => e.ApiName).HasMaxLength(500);
            entity.Property(e => e.ErrorFileName).HasMaxLength(500);
            entity.Property(e => e.HttpMethod).HasMaxLength(10);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.RequestTime).HasColumnType("datetime");
            entity.Property(e => e.ResponseTime).HasColumnType("datetime");
            entity.Property(e => e.Session).HasMaxLength(500);
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => new { e.BranchId, e.CmId });

            entity.ToTable("branch");

            entity.HasIndex(e => new { e.BranchId, e.ShortName, e.CmId }, "UK_Branch_Short_Name")
                .IsUnique()
                .HasFillFactor(80);

            entity.HasIndex(e => new { e.CmId, e.BranchName }, "UK_Comp_BranchName").IsUnique();

            entity.HasIndex(e => new { e.BranchId, e.CmId }, "UK_branch_Comp").IsUnique();

            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.CmId).HasColumnName("Cm_ID");
            entity.Property(e => e.AcHeadId)
                .HasDefaultValue(0)
                .HasColumnName("AcHeadID");
            entity.Property(e => e.AccountingBranch)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Accounting_Branch");
            entity.Property(e => e.ActionOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.BranchName).HasMaxLength(100);
            entity.Property(e => e.BranchRegionId)
                .HasDefaultValue(0)
                .HasColumnName("Branch_Region_ID");
            entity.Property(e => e.BranchTypeFk).HasColumnName("BranchType_FK");
            entity.Property(e => e.CardRate).HasColumnName("Card_rate");
            entity.Property(e => e.CbAdd1)
                .HasMaxLength(100)
                .HasColumnName("Cb_Add1");
            entity.Property(e => e.CbAdd2)
                .HasMaxLength(100)
                .HasColumnName("Cb_Add2");
            entity.Property(e => e.CbAdd3)
                .HasMaxLength(100)
                .HasColumnName("Cb_Add3");
            entity.Property(e => e.CbDesc)
                .HasMaxLength(40)
                .HasColumnName("Cb_desc");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Cno)
                .HasMaxLength(50)
                .HasColumnName("CNO");
            entity.Property(e => e.Cnodate)
                .HasColumnType("datetime")
                .HasColumnName("cnodate");
            entity.Property(e => e.CommisionRate).HasMaxLength(300);
            entity.Property(e => e.Commissionerate)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ContralBranchIdFk).HasColumnName("Contral_BranchID_FK");
            entity.Property(e => e.ContralBranchSrNo).HasColumnName("Contral_Branch_SrNo");
            entity.Property(e => e.CstRegDate)
                .HasColumnType("datetime")
                .HasColumnName("Cst_Reg_Date");
            entity.Property(e => e.CstTax)
                .HasMaxLength(150)
                .HasColumnName("Cst_tax");
            entity.Property(e => e.CtPerson)
                .HasMaxLength(100)
                .HasColumnName("Ct_Person");
            entity.Property(e => e.Des1).HasMaxLength(50);
            entity.Property(e => e.Des2).HasMaxLength(50);
            entity.Property(e => e.Division).HasMaxLength(100);
            entity.Property(e => e.EccDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("ECC_Date");
            entity.Property(e => e.EccNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ECC_No");
            entity.Property(e => e.EccregBranchId)
                .HasDefaultValue((short)0)
                .HasColumnName("ECCRegBranchID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.ExciseDivision)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Excise_Division");
            entity.Property(e => e.ExciseRangeOff)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Excise_Range_Off");
            entity.Property(e => e.FactoryLicenceNo)
                .HasMaxLength(50)
                .HasColumnName("Factory_LicenceNo");
            entity.Property(e => e.FaxNo)
                .HasMaxLength(50)
                .HasColumnName("Fax_No");
            entity.Property(e => e.GstRegMainBranch)
                .HasDefaultValue(false)
                .HasColumnName("GST_RegMainBranch");
            entity.Property(e => e.GstinNoSer)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("GSTIN_No_Ser");
            entity.Property(e => e.GstinNoSuply)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("GSTIN_No_Suply");
            entity.Property(e => e.IgstNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("IGST_No");
            entity.Property(e => e.InsuranceNo).HasMaxLength(50);
            entity.Property(e => e.LockDate).HasColumnType("datetime");
            entity.Property(e => e.Machine)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasDefaultValueSql("(host_name())");
            entity.Property(e => e.Mobile).HasMaxLength(15);
            entity.Property(e => e.NatureCodeFk).HasColumnName("Nature_Code_Fk");
            entity.Property(e => e.Notifdate)
                .HasColumnType("datetime")
                .HasColumnName("notifdate");
            entity.Property(e => e.Notification)
                .HasMaxLength(50)
                .HasColumnName("notification");
            entity.Property(e => e.PanNo)
                .HasMaxLength(150)
                .HasColumnName("Pan_No");
            entity.Property(e => e.PcenterIdFk).HasColumnName("PCenterID_Fk");
            entity.Property(e => e.Phone1).HasMaxLength(20);
            entity.Property(e => e.Phone2).HasMaxLength(20);
            entity.Property(e => e.Pin).HasMaxLength(10);
            entity.Property(e => e.PortCodeFk).HasColumnName("Port_Code_FK");
            entity.Property(e => e.Range).HasMaxLength(100);
            entity.Property(e => e.RegionRank)
                .HasDefaultValue((short)0)
                .HasColumnName("Region_Rank");
            entity.Property(e => e.RegistrationNo).HasMaxLength(100);
            entity.Property(e => e.RoundoffAcHeadId).HasColumnName("RoundoffAcHeadID");
            entity.Property(e => e.RptApproovedBy1)
                .HasMaxLength(80)
                .HasColumnName("Rpt_ApproovedBy1");
            entity.Property(e => e.RptApproovedBy2)
                .HasMaxLength(80)
                .HasColumnName("Rpt_ApproovedBy2");
            entity.Property(e => e.RptConfirmedBy)
                .HasMaxLength(80)
                .HasColumnName("Rpt_ConfirmedBy");
            entity.Property(e => e.ShortName)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("Short_Name");
            entity.Property(e => e.StTax)
                .HasMaxLength(150)
                .HasColumnName("St_tax");
            entity.Property(e => e.StateIdFk).HasColumnName("StateId_FK");
            entity.Property(e => e.TanNo)
                .HasMaxLength(150)
                .HasColumnName("Tan_No");
            entity.Property(e => e.TinNo)
                .HasMaxLength(150)
                .HasColumnName("Tin_No");
            entity.Property(e => e.TinRegDate)
                .HasColumnType("datetime")
                .HasColumnName("Tin_Reg_Date");
            entity.Property(e => e.Unit).HasDefaultValue((short)0);
            entity.Property(e => e.UserIdFk).HasColumnName("UserId_FK");
            entity.Property(e => e.WefRegBrDt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("wef_Reg_BrDt");
            entity.Property(e => e.Zone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ZoneCodeFk).HasColumnName("Zone_Code_FK");

            //entity.HasOne(d => d.BranchRegion).WithMany(p => p.Branches)
            //    .HasForeignKey(d => d.BranchRegionId)
            //    .HasConstraintName("FK_branch_Branch_Region_Mast");

            //entity.HasOne(d => d.PortCodeFkNavigation).WithMany(p => p.Branches)
            //    .HasForeignKey(d => d.PortCodeFk)
            //    .HasConstraintName("FK_branch_IMP_M_Port_Master");

            //entity.HasOne(d => d.StateIdFkNavigation).WithMany(p => p.Branches)
            //    .HasForeignKey(d => d.StateIdFk)
            //    .HasConstraintName("FK_branch_M_State_Master");

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.InverseBranchNavigation)
                .HasForeignKey(d => new { d.AccountingBranch, d.CmId })
                .HasConstraintName("FK_branch_Accounting_Branch");

            entity.HasOne(d => d.Branch1).WithMany(p => p.InverseBranch1)
                .HasForeignKey(d => new { d.EccregBranchId, d.CmId })
                .HasConstraintName("FK_branch_ECC_Branch");

            entity.HasOne(d => d.Branch2).WithMany(p => p.InverseBranch2)
                .HasForeignKey(d => new { d.HeadBranchId, d.CmId })
                .HasConstraintName("FK_branch_HeadBranchId");

            entity.HasOne(d => d.Branch3).WithMany(p => p.InverseBranch3)
                .HasForeignKey(d => new { d.RefBranchId, d.CmId })
                .HasConstraintName("FK_branch_RefBranchId");
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__UserSess__C9F49290DC6D1C37");

            entity.ToTable("UserSession");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LoginTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SessionToken).HasMaxLength(500);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.Cmid, e.Branchid });

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Cmid).HasColumnName("cmid");
            entity.Property(e => e.Branchid).HasColumnName("branchid");
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.DefaLoginSrNo).HasColumnName("defa_Login_SrNo");
            entity.Property(e => e.Department)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.EmpId).HasDefaultValue(0);
            entity.Property(e => e.EmpName).HasMaxLength(50);
            entity.Property(e => e.EncreptedPsw)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.FriendlyBranchId).HasColumnName("Friendly_BranchID");
            entity.Property(e => e.FriendlyCmId).HasColumnName("Friendly_Cm_ID");
            entity.Property(e => e.Index1).ValueGeneratedOnAdd();
            entity.Property(e => e.Login).HasDefaultValue(false);
            entity.Property(e => e.LoginAt).HasColumnType("datetime");
            entity.Property(e => e.Machine)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(12);
            entity.Property(e => e.Section)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(25);
            entity.Property(e => e.UserPass)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UserType)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.Branch).WithMany(p => p.Users)
                .HasForeignKey(d => new { d.Branchid, d.Cmid })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_users_branch");
        });

        modelBuilder.Entity<ApiLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ApiLog__3214EC07DCE397E0");

            entity.ToTable("ApiLog");

            entity.Property(e => e.ApiName).HasMaxLength(500);
            entity.Property(e => e.ErrorFileName).HasMaxLength(500);
            entity.Property(e => e.HttpMethod).HasMaxLength(10);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.RequestTime).HasColumnType("datetime");
            entity.Property(e => e.ResponseTime).HasColumnType("datetime");
            entity.Property(e => e.Session).HasMaxLength(500);
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
