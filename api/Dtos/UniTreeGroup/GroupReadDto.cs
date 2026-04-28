public class GroupReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CreatedByUserName { get; set; } = string.Empty;
    public decimal ContributionAmount { get; set; }
    public string PayoutCycle { get; set; } = string.Empty;
    public int MaxMembers { get; set; }
    public int CurrentMemberCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal PoolBalance { get; set; }
}