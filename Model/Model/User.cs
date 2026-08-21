using System;
using System.Collections.Generic;

namespace Model;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public short Cmid { get; set; }

    public short Branchid { get; set; }

    public string UserPass { get; set; } = null!;

    public string? EncreptedPsw { get; set; }

    public string? Name { get; set; }

    public int? EmpId { get; set; }

    public string? EmpName { get; set; }

    public string? EmailId { get; set; }

    public string UserType { get; set; } = null!;

    public bool FriendlyCmId { get; set; }

    public bool FriendlyBranchId { get; set; }

    public int? DefaLoginSrNo { get; set; }

    public bool? Active { get; set; }

    public bool? Login { get; set; }

    public DateTime? LoginAt { get; set; }

    public string? Machine { get; set; }

    public int Index1 { get; set; }

    public string? Department { get; set; }

    public short? InsertPer { get; set; }

    public short? EditPer { get; set; }

    public short? DeletePer { get; set; }

    public short? Reportper { get; set; }

    public short? ExportPer { get; set; }

    public short Usertypeid { get; set; }

    public short UserGroupId { get; set; }

    public string Section { get; set; } = null!;

    public string? Phone { get; set; }

    public virtual Branch Branch { get; set; } = null!;
}
