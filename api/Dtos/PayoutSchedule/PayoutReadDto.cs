public class PayoutReadDto
{
    public int Id { get; set; }
    public int BeneficiaryUserId { get; set; }
    public string BeneficiaryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
}