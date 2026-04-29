using Microsoft.EntityFrameworkCore;

public interface ITransactionsRepository : IGenericRepository<Transactions>
{
    Task<Transactions> CreateWithLedgerAsync(Transactions transaction, List<LedgerEntry> entries);
    Transactions? GetByReference(string reference);
    IEnumerable<Transactions> GetByUserId(int userId);
    Transactions? GetByIdWithUser(int id);
}

public class TransactionsRepository : GenericRepository<Transactions>, ITransactionsRepository
{
    public TransactionsRepository(UniTreeDbContext context) : base(context) { }

    public async Task<Transactions> CreateWithLedgerAsync(Transactions transaction, List<LedgerEntry> entries)
    {
        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try 
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync(); 

            foreach (var entry in entries)
            {
                entry.TransactionsId = transaction.Id; // Ensure FK is set
                _context.Set<LedgerEntry>().Add(entry);
            }

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();
            return transaction;
        }
        catch (DbUpdateConcurrencyException)
        {
            await dbTransaction.RollbackAsync();
            throw new InvalidOperationException("Wallet or group was modified by another transaction. Try again.");
        }
        catch (Exception)
        {
            await dbTransaction.RollbackAsync();
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

    public Transactions? GetByIdWithUser(int id)
    {
        return _context.Transactions.Include(t => t.User).FirstOrDefault(t => t.Id == id);
    }
}