using Microsoft.EntityFrameworkCore;

public class UniTreeDbContext : DbContext
{
    public UniTreeDbContext(DbContextOptions<UniTreeDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Membership> Memberships { get; set; } = null!;
    public DbSet<Wallet> Wallets { get; set; } = null!;
    public DbSet<Transactions> Transactions { get; set; } = null!;
    public DbSet<PayoutSchedule> PayoutSchedules { get; set; } = null!;
    public DbSet<UniTreeGroup> UniTreeGroups { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .UseIdentityColumn();
        
        modelBuilder.Entity<Transactions>()
            .Property(t => t.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<Membership>()
            .Property(m => m.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<UniTreeGroup>()
            .Property(g => g.Id)
            .UseIdentityColumn();

        // Relationship: Group -> Creator
        modelBuilder.Entity<UniTreeGroup>()
            .HasOne(g => g.CreatedBy)
            .WithMany()
            .HasForeignKey(g => g.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: Membership -> Group
        modelBuilder.Entity<Membership>()
            .HasOne(m => m.UniTreeGroup) 
            .WithMany(g => g.Memberships)
            .HasForeignKey(m => m.UniTreeGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Membership -> User
        modelBuilder.Entity<Membership>()
            .HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}