using Microsoft.AspNetCore.Mvc.Formatters.Xml;

public interface IWalletRepository : IRepositoryWrapper<Wallet>
{
    Wallet? GetByUserId(int userId);
    Task<Wallet?> GetByUserIdAsync(int userId);
}