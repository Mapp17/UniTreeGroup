using Microsoft.EntityFrameworkCore;

public class UserRepository
{
    private readonly UniTreeDbContext _context;
    public UserRepository(UniTreeDbContext context)
    {
        _context = context;
    }

    // New helper for existence checks
    public User? GetUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
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
        catch (DbUpdateException)
        {
            // Fallback for database-level unique constraint violations
            throw new BadRequestException("An error occurred while saving to the database. Ensure data is unique.");
        }
    }  
}