public interface IUnitOfWork : IDisposable
{
    UserRepository Users { get; }
    TransactionsRepository Transactions { get; }
    UniTreeGroupRepository Groups { get; }
    PayoutRepository Payouts { get; }
    IWalletRepository Wallets { get; }
    Task<int> CompleteAsync();
}