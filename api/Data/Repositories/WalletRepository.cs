using Microsoft.EntityFrameworkCore;

public class WalletRepository : RepositoryWrapper<Wallet>, IWalletRepository
{
    public WalletRepository(UniTreeDbContext context) : base(context) {}

    public Wallet? GetByUserId(int userId)
    {
        return _context.Wallets.FirstOrDefault(w => w.UserId == userId);
    }

    public async Task<Wallet?> GetByUserIdAsync(int userId)
    {
        return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
    }
}