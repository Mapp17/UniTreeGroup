public interface IUnitOfWork : IDisposable
{
    UserRepository Users { get; }
    TransactionsRepository Transactions { get; }
    UniTreeGroupRepository Groups { get; }
    PayoutRepository Payouts { get; }
    Task<int> CompleteAsync();
}