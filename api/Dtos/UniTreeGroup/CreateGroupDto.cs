public class CreateGroupDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CreatedById { get; set; }
    public decimal ContributionAmount { get; set; }
    public int PayoutCycle { get; set; } // Map to PayoutCycleType
    public int MaxMembers { get; set; }
}