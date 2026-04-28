using Microsoft.EntityFrameworkCore;

public class TransactionsRepository
{
    private readonly UniTreeDbContext _context;

    public TransactionsRepository(UniTreeDbContext context)
    {
        _context = context;
    }

    public Transactions CreateWithLedger(Transactions transaction, List<LedgerEntry> entries)
    {
        using var dbTransaction = _context.Database.BeginTransaction();
        try 
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges(); // Get Transaction ID

            foreach (var entry in entries)
            {
                // Ensure TransactionId is linked (assuming domain logic uses Id not Guid for simplicity now)
                _context.Set<LedgerEntry>().Add(entry);
            }

            _context.SaveChanges();
            dbTransaction.Commit();
            return transaction;
        }
        catch (Exception)
        {
            dbTransaction.Rollback();
            throw new ConflictException("Could not save transaction. A database or ledger conflict occurred.");
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