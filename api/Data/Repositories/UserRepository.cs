using Microsoft.EntityFrameworkCore;



public class UserRepository : RepositoryWrapper<User>, IUserRepository
{
    public UserRepository(UniTreeDbContext context) : base(context) { }

    public User? GetByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
    }

    public User? GetByIdWithWallet(int id)
    {
        return _context.Users.Include(u => u.Wallet)
                .FirstOrDefault(u => u.Id == id);
    }
}