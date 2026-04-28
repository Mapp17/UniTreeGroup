using Microsoft.EntityFrameworkCore;

public class UserRepository
{
    private readonly UniTreeDbContext _context;
    public UserRepository(UniTreeDbContext context)
    {
        _context = context;
    }

    public User? GetUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _context.Users.ToList();
    }

    public User? GetUserById(int id)
    {
        return _context.Users.Include(u => u.Wallet)
                .FirstOrDefault(u => u.Id == id);
    }

    public User CreateUser(User user)
    {
        try
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            // Pass the actual Postgres error to your BadRequest handler
            var realError = ex.InnerException?.Message ?? ex.Message;
            throw new BadRequestException($"Database save failed: {realError}");
        }
    }
}