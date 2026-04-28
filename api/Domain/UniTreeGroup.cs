
public class UniTreeGroup : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CreatedById { get; set; }
    public decimal ContributionAmount { get; set; }
    public PayoutCycleType PayoutCycle { get; set; }
    public int MaxMembers { get; set; }
    public StokvelStatus Status { get; set; } = StokvelStatus.Active;
    public decimal PoolBalance { get; set; } = 0m;

    // Navigation
    public User CreatedBy { get; set; } = null!;
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<PayoutSchedule> PayoutSchedules { get; set; } = new List<PayoutSchedule>();
    public ICollection<LedgerEntry> LedgerEntries { get; set; } = new List<LedgerEntry>();
}