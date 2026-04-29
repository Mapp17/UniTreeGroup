using Microsoft.EntityFrameworkCore;

public class TransactionsServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UniTreeDbContext _context;

    public TransactionsServices(IUnitOfWork unitOfWork, UniTreeDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<TransactionReadDto> ProcessDeposit(TransactionRequestDto dto)
        => await ProcessTransaction(dto, TransactionType.Deposit);

    public async Task<TransactionReadDto> ProcessWithdraw(TransactionRequestDto dto)
        => await ProcessTransaction(dto, TransactionType.Withdrawal);

    public async Task<TransactionReadDto> ProcessContribute(TransactionRequestDto dto)
        => await ProcessTransaction(dto, TransactionType.Contribution);

    public async Task<TransactionReadDto> ProcessPayout(TransactionRequestDto dto)
        => await ProcessTransaction(dto, TransactionType.Payout);

    private async Task<TransactionReadDto> ProcessTransaction(TransactionRequestDto dto, TransactionType type)
    {
        // 1. GUARD: Basic validations
        if (dto.Amount <= 0)
            throw new BadRequestException("Transaction amount must be greater than zero.");

        if (_unitOfWork.Transactions.GetByReference(dto.Reference) != null)
            throw new ConflictException("A transaction with this reference already exists.");

        // RELOAD wallet to ensure we have the latest balance and RowVersion
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == dto.UserId);
        
        if (wallet == null) throw new NotFoundException($"Wallet for user {dto.UserId} not found.");

        var user = _unitOfWork.Users.GetByIdWithWallet(dto.UserId);
        if (user == null) throw new NotFoundException($"User {dto.UserId} not found.");

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
                wallet.Balance += dto.Amount;
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Debit, AccountName = "ExternalBank", Amount = dto.Amount, Description = "External Deposit", Reference = dto.Reference });
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Credit, AccountName = "UserWallet", WalletId = wallet.Id, Amount = dto.Amount, Description = $"Deposit for {user.FullName}", Reference = dto.Reference });
                break;

            case TransactionType.Withdrawal:
                if (wallet.Balance < dto.Amount) throw new BadRequestException("Insufficient funds.");
                wallet.Balance -= dto.Amount;
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Debit, AccountName = "UserWallet", WalletId = wallet.Id, Amount = dto.Amount, Description = "Wallet Withdrawal", Reference = dto.Reference });
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Credit, AccountName = "ExternalBank", Amount = dto.Amount, Description = "External Transfer", Reference = dto.Reference });
                break;

            case TransactionType.Contribution:
                if (!dto.UniTreeGroupId.HasValue) throw new BadRequestException("Group ID is required for contributions.");
                var group = await _unitOfWork.Groups.GetByIdWithDetailsAsync(dto.UniTreeGroupId.Value);
                if (group == null) throw new NotFoundException("Stokvel Group not found.");

                var membership = group.Memberships.FirstOrDefault(m => m.UserId == user.Id);
                if (membership == null) throw new BadRequestException("User is not a member of this group.");

                if (wallet.Balance < dto.Amount) throw new BadRequestException("Insufficient funds. Please deposit to your wallet first.");
                
                wallet.Balance -= dto.Amount;
                group.PoolBalance += dto.Amount;
                membership.TotalContributed += dto.Amount;

                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Debit, AccountName = "UserWallet", WalletId = wallet.Id, Amount = dto.Amount, Description = $"Contribution to {group.Name}", Reference = dto.Reference });
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Credit, AccountName = "StokvelPool", StokvelGroupId = group.Id, Amount = dto.Amount, Description = $"Contribution from {user.FullName}", Reference = dto.Reference });
                break;

            case TransactionType.Payout:
                if (!dto.UniTreeGroupId.HasValue) throw new BadRequestException("Group ID is required for payouts.");
                var pGroup = await _unitOfWork.Groups.GetByIdWithDetailsAsync(dto.UniTreeGroupId.Value);
                if (pGroup == null) throw new NotFoundException("Stokvel Group not found.");

                if (pGroup.PoolBalance < dto.Amount) throw new BadRequestException("Insignificant pool funds.");

                pGroup.PoolBalance -= dto.Amount;
                wallet.Balance += dto.Amount;

                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Debit, AccountName = "StokvelPool", StokvelGroupId = pGroup.Id, Amount = dto.Amount, Description = $"Payout to {user.FullName}", Reference = dto.Reference });
                ledgerEntries.Add(new LedgerEntry { EntryType = LedgerEntryType.Credit, AccountName = "UserWallet", WalletId = wallet.Id, Amount = dto.Amount, Description = "Group Payout Received", Reference = dto.Reference });
                break;
        }

        // 4. PERSIST
        var created = await _unitOfWork.Transactions.CreateWithLedgerAsync(transaction, ledgerEntries);
        return MapToDto(created);
    }

    public IEnumerable<TransactionReadDto> GetUserHistory(int userId)
    {
        if (_unitOfWork.Users.GetByIdWithWallet(userId) == null)
            throw new NotFoundException($"User {userId} not found.");
        return _unitOfWork.Transactions.GetByUserId(userId).Select(MapToDto);
    }

    public TransactionReadDto GetLedgerEntry(int id)
    {
        var transaction = _unitOfWork.Transactions.GetByIdWithUser(id);
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