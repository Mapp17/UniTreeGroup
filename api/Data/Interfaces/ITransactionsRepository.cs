public interface ITransactionsRepository : IRepositoryWrapper<Transactions>
{
    Task<Transactions> CreateWithLedgerAsync(Transactions transaction, List<LedgerEntry> entries);
    Transactions? GetByReference(string reference);
    IEnumerable<Transactions> GetByUserId(int userId);
    Transactions? GetByIdWithUser(int id);
}