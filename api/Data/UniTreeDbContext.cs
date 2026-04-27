using Microsoft.EntityFrameworkCore;

public class UniTreeDbContext : DbContext
{
    public UniTreeDbContext(DbContextOptions<UniTreeDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Membership> Memberships { get; set; } = null!;
    public DbSet<Wallet> Wallets { get; set; } = null!;
    public DbSet<Transactions> Transactions { get; set; } = null!;
    public DbSet<PayoutSchedule> PayoutSchedules { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .UseIdentityColumn(); // Specifically tells Npgsql to use an auto-incrementing column
    }
}