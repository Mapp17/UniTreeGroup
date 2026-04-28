
public class TransactionsServices
{
    private readonly TransactionsRepository _repo;
    private readonly UserRepository _userRepo;

    public TransactionsServices(TransactionsRepository repo, UserRepository userRepo)
    {
        _repo = repo;
        _userRepo = userRepo;
    }

    public TransactionReadDto ProcessDeposit(TransactionRequestDto dto)
        => ProcessTransaction(dto, TransactionType.Deposit);

    public TransactionReadDto ProcessWithdraw(TransactionRequestDto dto)
        => ProcessTransaction(dto, TransactionType.Withdrawal);

    public TransactionReadDto ProcessContribute(TransactionRequestDto dto)
        => ProcessTransaction(dto, TransactionType.Contribution);

    private TransactionReadDto ProcessTransaction(TransactionRequestDto dto, TransactionType type)
    {
        // 1. GUARD: Amount must be positive
        if (dto.Amount <= 0)
            throw new BadRequestException("Transaction amount must be greater than zero.");

        // 2. GUARD: Check for duplicate reference (Idempotency)
        if (_repo.GetByReference(dto.Reference) != null)
            throw new ConflictException("A transaction with this reference already exists.");

        // 3. GUARD: User and Wallet check
        var user = _userRepo.GetUserById(dto.UserId);
        if (user == null) throw new NotFoundException($"User {dto.UserId} not found.");
        if (user.Wallet == null) throw new BadRequestException("User does not have an active wallet.");

        // 4. GUARD: Sufficient funds for outflows
        if (type == TransactionType.Withdrawal || type == TransactionType.Contribution)
        {
            if (user.Wallet.Balance < dto.Amount)
                throw new BadRequestException("Insignificant funds to complete this transaction.");

            user.Wallet.Balance -= dto.Amount; // Deduct funds
        }
        else if (type == TransactionType.Deposit)
        {
            user.Wallet.Balance += dto.Amount; // Add funds
        }

        var transaction = new Transactions
        {
            UserId = dto.UserId,
            Type = type,
            Amount = dto.Amount,
            Reference = dto.Reference,
            Description = dto.Description,
            Status = TransactionStatus.Completed // Assuming instant processing for this logic
        };

        var created = _repo.Create(transaction);
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
