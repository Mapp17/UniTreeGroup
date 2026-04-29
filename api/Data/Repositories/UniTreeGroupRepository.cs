using Microsoft.EntityFrameworkCore;

public interface IUniTreeGroupRepository : IGenericRepository<UniTreeGroup>
{
    Task<IEnumerable<UniTreeGroup>> GetAllWithDetailsAsync();
    Task<UniTreeGroup?> GetByIdWithDetailsAsync(int id);
    Task<bool> IsUserInAnyGroupAsync(int userId);
    Task<bool> AddMemberAsync(Membership membership);
}

public class UniTreeGroupRepository : GenericRepository<UniTreeGroup>, IUniTreeGroupRepository
{
    public UniTreeGroupRepository(UniTreeDbContext context) : base(context) { }

    public async Task<IEnumerable<UniTreeGroup>> GetAllWithDetailsAsync()
    {
        return await _context.UniTreeGroups
            .Include(g => g.CreatedBy)
            .Include(g => g.Memberships)
            .ToListAsync();
    }

    public async Task<UniTreeGroup?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.UniTreeGroups
            .Include(g => g.CreatedBy)
            .Include(g => g.Memberships)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<bool> IsUserInAnyGroupAsync(int userId)
    {
        return await _context.Memberships
            .AnyAsync(m => m.UserId == userId);
    }

    public async Task<bool> AddMemberAsync(Membership membership)
    {
        try 
        {
            await _context.Memberships.AddAsync(membership);
            return true; // UnitOfWork.CompleteAsync will handle the SaveChanges
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        {
            throw new ConflictException("Could not join group. This user might already belong to a group.", dbEx.InnerException?.Message);
        }
    }
}