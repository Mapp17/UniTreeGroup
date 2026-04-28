
public class Membership : BaseModel
{
    public int UserId { get; set; }
    public int UniTreeGroupId { get; set; } 
    public MembershipStatus Status { get; set; } = MembershipStatus.Active;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public int PayoutOrder { get; set; } 
    public decimal TotalContributed { get; set; } = 0m;
    public bool HasReceivedPayout { get; set; } = false;

    // Navigation
    public User User { get; set; } = null!;
    public UniTreeGroup UniTreeGroup { get; set; } = null!; // Added this
}