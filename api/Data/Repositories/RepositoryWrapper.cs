using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class RepositoryWrapper<T> : IRepositoryWrapper<T> where T : BaseModel
{
    protected readonly UniTreeDbContext _context;
    public RepositoryWrapper(UniTreeDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

    public IEnumerable<T> Find(Expression<Func<T, bool>> predicate) => _context.Set<T>().Where(predicate).ToList();

    public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

    public void Update(T entity) => _context.Set<T>().Update(entity);

    public void Remove(T entity) => _context.Set<T>().Remove(entity);
}