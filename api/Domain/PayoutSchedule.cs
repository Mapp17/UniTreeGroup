

public class PayoutSchedule : BaseModel
{

    public int BeneficiaryUserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public PayoutScheduleStatus Status { get; set; } = PayoutScheduleStatus.Scheduled;
    public Guid? TransactionId { get; set; }

    // Navigation
    public User BeneficiaryUser { get; set; } = null!;
    public Transactions? Transaction { get; set; }
}