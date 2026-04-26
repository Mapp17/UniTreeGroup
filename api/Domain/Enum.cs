

public enum TransactionType
{
    Deposit,
    Withdrawal,
    Contribution,
    Payout,
    Transfer
}

public enum TransactionStatus
{
    Pending,
    Completed,
    Failed,
    Reversed
}

public enum LedgerEntryType
{
    Debit,
    Credit
}

public enum MembershipStatus
{
    Active,
    Suspended,
    Left
}

public enum PayoutCycleType
{
    Monthly,
    Quarterly,
    Annually,
    Custom
}

public enum PayoutScheduleStatus
{
    Scheduled,
    Completed,
    Skipped
}

public enum StokvelStatus
{
    Active,
    Paused,
    Closed
}