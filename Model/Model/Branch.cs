using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{

    public partial class Branch
    {
        public short BranchId { get; set; }

        public short SrNo { get; set; }

        public short CmId { get; set; }

        public string BranchName { get; set; } = null!;

        public string ShortName { get; set; } = null!;

        public short? BranchTypeFk { get; set; }

        public short? RefBranchId { get; set; }

        public short? HeadBranchId { get; set; }

        public string? CbAdd1 { get; set; }

        public string? CbAdd2 { get; set; }

        public string? CbAdd3 { get; set; }

        public string? City { get; set; }

        public string? Pin { get; set; }

        public string? Phone1 { get; set; }

        public string? Phone2 { get; set; }

        public string? FaxNo { get; set; }

        public string? CtPerson { get; set; }

        public string? Mobile { get; set; }

        public string? Email { get; set; }

        public string? StTax { get; set; }

        public string? CstTax { get; set; }

        public string? TinNo { get; set; }

        public string? TanNo { get; set; }

        public string? PanNo { get; set; }

        public short? StateIdFk { get; set; }

        public short? ZoneCodeFk { get; set; }

        public int? AcHeadId { get; set; }

        public bool AutoEntry { get; set; }

        public short Rank { get; set; }

        public int UserIdFk { get; set; }

        public DateTime ActionOn { get; set; }

        public string Machine { get; set; } = null!;

        public short? RegionRank { get; set; }

        public string? Des1 { get; set; }

        public string? Des2 { get; set; }

        public double? CardRate { get; set; }

        public int? RoundoffAcHeadId { get; set; }

        public string? InsuranceNo { get; set; }

        public string? Division { get; set; }

        public string? Range { get; set; }

        public string? RegistrationNo { get; set; }

        public string? CommisionRate { get; set; }

        public DateTime? LockDate { get; set; }

        public string? Cno { get; set; }

        public DateTime? Cnodate { get; set; }

        public string? Notification { get; set; }

        public DateTime? Notifdate { get; set; }

        public string? Zone { get; set; }

        public short? ContralBranchIdFk { get; set; }

        public int ContralBranchSrNo { get; set; }

        public short? Unit { get; set; }

        public short? BrGroup { get; set; }

        public short? PortCodeFk { get; set; }

        public string? CbDesc { get; set; }

        public short? NatureCodeFk { get; set; }

        public string? RptConfirmedBy { get; set; }

        public string? RptApproovedBy1 { get; set; }

        public string? RptApproovedBy2 { get; set; }

        public DateTime? CstRegDate { get; set; }

        public DateTime? TinRegDate { get; set; }

        public string? ExciseRangeOff { get; set; }

        public string? ExciseDivision { get; set; }

        public string? Commissionerate { get; set; }

        public string? EccNo { get; set; }

        public int? PcenterIdFk { get; set; }

        public DateTime? EccDate { get; set; }

        public int? BranchRegionId { get; set; }

        public short? EccregBranchId { get; set; }

        public short? AccountingBranch { get; set; }

        public string? FactoryLicenceNo { get; set; }

        public bool Active { get; set; }

        public DateTime? WefRegBrDt { get; set; }

        public short? BudgetBranch { get; set; }

        public string? GstinNoSuply { get; set; }

        public string? GstinNoSer { get; set; }

        public string? IgstNo { get; set; }

        public bool? GstRegMainBranch { get; set; }

        public virtual Branch? Branch1 { get; set; }

        public virtual Branch? Branch2 { get; set; }

        public virtual Branch? Branch3 { get; set; }

        //public virtual ICollection<BranchContactDetail> BranchContactDetails { get; set; } = new List<BranchContactDetail>();

        //public virtual ICollection<BranchDealArea> BranchDealAreas { get; set; } = new List<BranchDealArea>();

        public virtual Branch? BranchNavigation { get; set; }

        //public virtual BranchRegionMast? BranchRegion { get; set; }

        public virtual ICollection<Branch> InverseBranch1 { get; set; } = new List<Branch>();

        public virtual ICollection<Branch> InverseBranch2 { get; set; } = new List<Branch>();

        public virtual ICollection<Branch> InverseBranch3 { get; set; } = new List<Branch>();

        public virtual ICollection<Branch> InverseBranchNavigation { get; set; } = new List<Branch>();

        //public virtual ICollection<MktTCostSheetM> MktTCostSheetMs { get; set; } = new List<MktTCostSheetM>();

        //public virtual ImpMPortMaster? PortCodeFkNavigation { get; set; }

        //public virtual MStateMaster? StateIdFkNavigation { get; set; }

        //public virtual ICollection<StkTOutward> StkTOutwards { get; set; } = new List<StkTOutward>();

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
