public class CreatePayoutDto
{
    public int BeneficiaryUserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ScheduledDate { get; set; }
}