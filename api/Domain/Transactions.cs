

/// <summary>
/// Represents a high-level financial event (deposit, withdrawal, contribution, payout).
/// Each Transaction produces two LedgerEntries (double-entry bookkeeping).
/// </summary>
public class Transactions : BaseModel
{
    public int UserId { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string Reference { get; set; } = string.Empty; // Idempotency key
    public string? Description { get; set; }
    public string? ExternalReference { get; set; } // From payment gateway

    // Navigation
    public User User { get; set; } = null!;
}
