public class UserRepository
{
    private readonly UniTreeDbContext _context;
    public UserRepository(UniTreeDbContext context)
    {
        _context = context;
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _context.Users.ToList();
    }

    public User? GetUserById(int id)
    {
        return _context.Users.Find(id);
    }

    public void AddUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }  

    

}