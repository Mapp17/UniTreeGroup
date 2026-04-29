using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class UniTreeDbContext : DbContext
{
    public UniTreeDbContext(DbContextOptions<UniTreeDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Membership> Memberships { get; set; } = null!;
    public DbSet<Wallet> Wallets { get; set; } = null!;
    public DbSet<Transactions> Transactions { get; set; } = null!;
    public DbSet<PayoutSchedule> PayoutSchedules { get; set; } = null!;
    public DbSet<UniTreeGroup> UniTreeGroups { get; set; } = null!;
    public DbSet<LedgerEntry> LedgerEntries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 🛠️ 1. ENABLE POSTGRES EXTENSIONS
        // This allows Postgres to use the 'gen_random_bytes' function
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<User>().Property(u => u.Id).UseIdentityColumn();
        modelBuilder.Entity<Transactions>().Property(t => t.Id).UseIdentityColumn();
        modelBuilder.Entity<Membership>().Property(m => m.Id).UseIdentityColumn();
        modelBuilder.Entity<UniTreeGroup>().Property(g => g.Id).UseIdentityColumn();
        modelBuilder.Entity<LedgerEntry>().Property(l => l.Id).UseIdentityColumn();

        // 🛠️ 2. CONCURRENCY TOKENS
        modelBuilder.Entity<Wallet>()
            .Property(w => w.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        modelBuilder.Entity<UniTreeGroup>()
            .Property(g => g.PoolBalance)
            .IsConcurrencyToken();

        // 🛠️ 3. PROVIDER-SPECIFIC ROWVERSION DEFAULTS
        if (Database.IsNpgsql())
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var rowVersionProp = entityType.FindProperty(nameof(BaseModel.RowVersion));
                if (rowVersionProp != null)
                {
                    rowVersionProp.SetDefaultValueSql("gen_random_bytes(8)");
                }
            }
        }
        else if (Database.IsSqlite())
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var rowVersionProp = entityType.FindProperty(nameof(BaseModel.RowVersion));
                if (rowVersionProp != null)
                {
                    rowVersionProp.SetDefaultValueSql("randomblob(8)");
                }
            }
        }

        // 🛠️ 4. RELATIONSHIPS
        modelBuilder.Entity<UniTreeGroup>()
            .HasOne(g => g.CreatedBy)
            .WithMany()
            .HasForeignKey(g => g.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Membership>()
            .HasOne(m => m.UniTreeGroup) 
            .WithMany(g => g.Memberships)
            .HasForeignKey(m => m.UniTreeGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Membership>()
            .HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 🛠️ 5. SOFT DELETE FILTERS
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var body = Expression.Equal(
                    Expression.Property(parameter, nameof(BaseModel.IsDeleted)),
                    Expression.Constant(false)
                );
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));
            }
        }
    }

    public override int SaveChanges()
    {
        HandleSoftDelete();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleSoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void HandleSoftDelete()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseModel && (e.State == EntityState.Deleted || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseModel)entry.Entity;
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
            }
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}