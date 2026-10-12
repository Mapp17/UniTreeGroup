using Microsoft.EntityFrameworkCore;



public class UserRepository : RepositoryWrapper<User>, IUserRepository
{
    public UserRepository(UniTreeDbContext context) : base(context) { }

    public User? GetByEmail(string email)
    {
        return _context.UniTreeUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
    }

    public User? GetByIdWithWallet(int id)
    {
        return _context.UniTreeUsers.Include(u => u.Wallet)
                .FirstOrDefault(u => u.Id == id);
    }
}