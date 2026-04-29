
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
    public ICollection<LedgerEntry> LedgerEntries { get; set; } = new List<LedgerEntry>();
}



public class LedgerEntry : BaseModel
{
    public int TransactionsId { get; set; } 
    public LedgerEntryType EntryType { get; set; } // Debit or Credit
    public string AccountName { get; set; } = string.Empty; 
    public int? WalletId { get; set; }
    public int? StokvelGroupId { get; set; }
    public string Reference { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
 
    // Navigation
    public Transactions Transactions { get; set; } = null!;
    public Wallet? Wallet { get; set; }
    public UniTreeGroup? StokvelGroup { get; set; }
}