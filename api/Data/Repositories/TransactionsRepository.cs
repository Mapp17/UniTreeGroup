using Microsoft.EntityFrameworkCore;

public class TransactionsRepository
{
    private readonly UniTreeDbContext _context;

    public TransactionsRepository(UniTreeDbContext context)
    {
        _context = context;
    }

    public Transactions Create(Transactions transaction)
{
    try 
    {
        _context.Transactions.Add(transaction);
        _context.SaveChanges();
        return transaction;
    }
    catch (Microsoft.EntityFrameworkCore.DbUpdateException)
    {
        // We turn a messy DB error into a clean "Conflict" error
        throw new ConflictException("Could not save transaction. The reference or ID might already exist.");
    }
}

    public Transactions? GetByReference(string reference)
    {
        return _context.Transactions.FirstOrDefault(t => t.Reference == reference);
    }

    public IEnumerable<Transactions> GetByUserId(int userId)
    {
        return _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToList();
    }

    public Transactions? GetById(int id)
    {
        return _context.Transactions.Include(t => t.User).FirstOrDefault(t => t.Id == id);
    }
}