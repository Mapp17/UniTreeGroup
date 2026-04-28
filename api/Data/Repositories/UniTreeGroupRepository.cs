using Microsoft.EntityFrameworkCore;

public class UniTreeGroupRepository
{
    private readonly UniTreeDbContext _context;

    public UniTreeGroupRepository(UniTreeDbContext context)
    {
        _context = context;
    }

    public UniTreeGroup Create(UniTreeGroup group)
    {
        try
        {
            _context.UniTreeGroups.Add(group);
            _context.SaveChanges();
            return group;
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        {
            throw new ConflictException("Could not create group. A database constraint was violated.", dbEx.InnerException?.Message);
        }
    }

    public IEnumerable<UniTreeGroup> GetAll()
    {
        return _context.UniTreeGroups
            .Include(g => g.CreatedBy)
            .Include(g => g.Memberships)
            .ToList();
    }

    public UniTreeGroup? GetById(int id)
    {
        return _context.UniTreeGroups
            .Include(g => g.CreatedBy)
            .Include(g => g.Memberships)
                .ThenInclude(m => m.User)
            .FirstOrDefault(g => g.Id == id);
    }

    public void AddMember(Membership membership)
    {
        _context.Memberships.Add(membership);
        _context.SaveChanges();
    }

    public async Task<UniTreeGroup?> GetByIdAsync(int id)
    {
        return await _context.UniTreeGroups
            .Include(g => g.CreatedBy)
            .Include(g => g.Memberships)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    // UPDATED: Check if user is in ANY group to prevent the 409 DB error
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
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        {
            throw new ConflictException("Could not join group. This user might already belong to a group.", dbEx.InnerException?.Message);
        }
    }
}