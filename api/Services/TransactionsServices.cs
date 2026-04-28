
public class TransactionsServices
{
    private readonly TransactionsRepository _repo;
    private readonly UserRepository _userRepo;
    private readonly UniTreeGroupRepository _groupRepo; // Added to manage Pool balances

    public TransactionsServices(TransactionsRepository repo, UserRepository userRepo, UniTreeGroupRepository groupRepo)
    {
        _repo = repo;
        _userRepo = userRepo;
        _groupRepo = groupRepo;
    }

    public TransactionReadDto ProcessDeposit(TransactionRequestDto dto)
        => ProcessTransaction(dto, TransactionType.Deposit);

    public TransactionReadDto ProcessWithdraw(TransactionRequestDto dto)
        => ProcessTransaction(dto, TransactionType.Withdrawal);

    public TransactionReadDto ProcessContribute(TransactionRequestDto dto)
        => ProcessTransaction(dto, TransactionType.Contribution);

    public TransactionReadDto ProcessPayout(TransactionRequestDto dto)
        => ProcessTransaction(dto, TransactionType.Payout);

    private TransactionReadDto ProcessTransaction(TransactionRequestDto dto, TransactionType type)
    {
        // 1. GUARD: Basic validations
        if (dto.Amount <= 0)
            throw new BadRequestException("Transaction amount must be greater than zero.");

        if (_repo.GetByReference(dto.Reference) != null)
            throw new ConflictException("A transaction with this reference already exists.");

        var user = _userRepo.GetUserById(dto.UserId);
        if (user == null) throw new NotFoundException($"User {dto.UserId} not found.");
        if (user.Wallet == null) throw new BadRequestException("User does not have an active wallet.");

        // 2. PREPARE LEDGER ENTRIES
        var ledgerEntries = new List<LedgerEntry>();
        var transaction = new Transactions
        {
            UserId = dto.UserId,
            Type = type,
            Amount = dto.Amount,
            Reference = dto.Reference,
            Description = dto.Description,
            Status = TransactionStatus.Completed
        };

        // 3. LOGIC PER TRANSACTION TYPE
        switch (type)
        {
            case TransactionType.Deposit:
                user.Wallet.Balance += dto.Amount;
                // Debit: External/Bank, Credit: User Wallet
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Debit, AccountName = "ExternalBank", Amount = dto.Amount, Description = "External Deposit" });
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Credit, AccountName = "UserWallet", WalletId = user.Wallet.Id, Amount = dto.Amount, Description = $"Deposit for {user.FullName}" });
                break;

            case TransactionType.Withdrawal:
                if (user.Wallet.Balance < dto.Amount) throw new BadRequestException("Insignificant funds.");
                user.Wallet.Balance -= dto.Amount;
                // Debit: User Wallet, Credit: External/Bank
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Debit, AccountName = "UserWallet", WalletId = user.Wallet.Id, Amount = dto.Amount, Description = "Wallet Withdrawal" });
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Credit, AccountName = "ExternalBank", Amount = dto.Amount, Description = "External Transfer" });
                break;

            case TransactionType.Contribution:
                if (!dto.UniTreeGroupId.HasValue) throw new BadRequestException("Group ID is required for contributions.");
                var group = _groupRepo.GetById(dto.UniTreeGroupId.Value);
                if (group == null) throw new NotFoundException("Stokvel Group not found.");

                if (user.Wallet.Balance < dto.Amount) throw new BadRequestException("Insignificant funds.");
                
                user.Wallet.Balance -= dto.Amount;
                group.PoolBalance += dto.Amount;

                // DOUBLE-ENTRY: Debit User Wallet, Credit Stokvel Pool
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Debit, AccountName = "UserWallet", WalletId = user.Wallet.Id, Amount = dto.Amount, Description = $"Contribution to {group.Name}" });
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Credit, AccountName = "StokvelPool", StokvelGroupId = group.Id, Amount = dto.Amount, Description = $"Contribution from {user.FullName}" });
                break;

            case TransactionType.Payout:
                if (!dto.UniTreeGroupId.HasValue) throw new BadRequestException("Group ID is required for payouts.");
                var pGroup = _groupRepo.GetById(dto.UniTreeGroupId.Value);
                if (pGroup == null) throw new NotFoundException("Stokvel Group not found.");

                if (pGroup.PoolBalance < dto.Amount) throw new BadRequestException("Insignificant pool funds.");

                pGroup.PoolBalance -= dto.Amount;
                user.Wallet.Balance += dto.Amount;

                // DOUBLE-ENTRY: Debit Stokvel Pool, Credit User Wallet
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Debit, AccountName = "StokvelPool", StokvelGroupId = pGroup.Id, Amount = dto.Amount, Description = $"Payout to {user.FullName}" });
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Credit, AccountName = "UserWallet", WalletId = user.Wallet.Id, Amount = dto.Amount, Description = "Group Payout Received" });
                break;
        }

        // 4. PERSIST (Atomic transaction)
        var created = _repo.CreateWithLedger(transaction, ledgerEntries);
        return MapToDto(created);
    }

    public IEnumerable<TransactionReadDto> GetUserHistory(int userId)
    {
        if (_userRepo.GetUserById(userId) == null)
            throw new NotFoundException($"User {userId} not found.");
        return _repo.GetByUserId(userId).Select(MapToDto);
    }

    public TransactionReadDto GetLedgerEntry(int id)
    {
        var transaction = _repo.GetById(id);
        if (transaction == null) throw new NotFoundException("Transaction record not found.");
        return MapToDto(transaction);
    }

    private static TransactionReadDto MapToDto(Transactions t) => new TransactionReadDto
    {
        Id = t.Id,
        UserId = t.UserId,
        Type = t.Type.ToString(),
        Status = t.Status.ToString(),
        Amount = t.Amount,
        Currency = t.Currency,
        Reference = t.Reference,
        Description = t.Description,
        CreatedAt = t.CreatedAt
    };
}
