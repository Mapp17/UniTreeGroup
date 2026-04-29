public interface IUnitOfWork : IDisposable
{
    UserRepository Users { get; }
    TransactionsRepository Transactions { get; }
    UniTreeGroupRepository Groups { get; }
    PayoutRepository Payouts { get; }
    Task<int> CompleteAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly UniTreeDbContext _context;

    public UnitOfWork(UniTreeDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Transactions = new TransactionsRepository(_context);
        Groups = new UniTreeGroupRepository(_context);
        Payouts = new PayoutRepository(_context);
    }

    public UserRepository Users { get; private set; }
    public TransactionsRepository Transactions { get; private set; }
    public UniTreeGroupRepository Groups { get; private set; }
    public PayoutRepository Payouts { get; private set; }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}