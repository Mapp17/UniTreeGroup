using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class FinancialFlowTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ServiceProvider _serviceProvider;
    private readonly UniTreeDbContext _context;
    private readonly TransactionsServices _transactionsService;

    public FinancialFlowTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _serviceProvider = new ServiceCollection()
            .AddDbContext<UniTreeDbContext>(options =>
                options.UseSqlite(_connection))
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<UniTreeGroupRepository>()
            .AddScoped<UserRepository>()
            .AddScoped<TransactionsRepository>()
            .AddScoped<UniTreeGroupServices>()
            .AddScoped<TransactionsServices>()
            .BuildServiceProvider();

        _context = _serviceProvider.GetRequiredService<UniTreeDbContext>();
        _context.Database.EnsureCreated();
        _transactionsService = _serviceProvider.GetRequiredService<TransactionsServices>();
    }

    [Fact]
    public async Task ProcessContribution_Should_Maintain_DoubleEntry_Integrity()
    {
        // ARRANGE
        var user = new User 
        { 
            Id = 10, 
            FullName = "Test User", 
            Email = "test@finance.com"
        };

        var wallet = new Wallet 
        { 
            Id = 10, 
            UserId = 10, 
            Balance = 1000m
        };

        var group = new UniTreeGroup 
        { 
            Id = 10, 
            Name = "Test Group", 
            PoolBalance = 0m, 
            CreatedById = 10 
        };

        var membership = new Membership 
        { 
            UserId = 10, 
            UniTreeGroupId = 10 
        };

        _context.Users.Add(user);
        _context.Wallets.Add(wallet);
        _context.UniTreeGroups.Add(group);
        _context.Memberships.Add(membership);

        await _context.SaveChangesAsync();

        var request = new TransactionRequestDto
        {
            UserId = 10,
            UniTreeGroupId = 10,
            Amount = 400m,
            Reference = "TEST-CONTRIB-001",
            Description = "Test contribution"
        };

        // ACT
        await _transactionsService.ProcessContribute(request);

        _context.ChangeTracker.Clear();

        // ASSERT
        var updatedWallet = await _context.Wallets.FindAsync(10);
        var updatedGroup = await _context.UniTreeGroups.FindAsync(10);

        Assert.NotNull(updatedWallet);
        Assert.NotNull(updatedGroup);

        var ledgerEntries = await _context.LedgerEntries
            .Where(l => l.Reference == "TEST-CONTRIB-001")
            .ToListAsync();

        // Balance checks
        Assert.Equal(600m, updatedWallet.Balance);
        Assert.Equal(400m, updatedGroup.PoolBalance);

        // Ledger integrity
        Assert.Equal(2, ledgerEntries.Count);

        var debit = ledgerEntries.FirstOrDefault(l => l.EntryType == LedgerEntryType.Debit);
        var credit = ledgerEntries.FirstOrDefault(l => l.EntryType == LedgerEntryType.Credit);

        Assert.NotNull(debit);
        Assert.NotNull(credit);

        Assert.Equal(400m, debit.Amount);
        Assert.Equal("UserWallet", debit.AccountName);

        Assert.Equal(400m, credit.Amount);
        Assert.Equal("StokvelPool", credit.AccountName);

        // Double-entry validation
        var totalDebits = ledgerEntries
            .Where(l => l.EntryType == LedgerEntryType.Debit)
            .Sum(l => l.Amount);

        var totalCredits = ledgerEntries
            .Where(l => l.EntryType == LedgerEntryType.Credit)
            .Sum(l => l.Amount);

        Assert.Equal(totalDebits, totalCredits);
    }

    [Fact]
    public async Task ProcessContribution_Should_Prevent_DoubleSpending()
    {
        // ARRANGE
        var user = new User { Id = 50, FullName = "Race User", Email = "race@test.com" };
        var wallet = new Wallet { Id = 50, UserId = 50, Balance = 500m };
        var group = new UniTreeGroup { Id = 50, Name = "Race Group", PoolBalance = 0m, CreatedById = 50 };
        var membership = new Membership { UserId = 50, UniTreeGroupId = 50 };

        _context.AddRange(user, wallet, group, membership);
        await _context.SaveChangesAsync();

        var request = new TransactionRequestDto
        {
            UserId = 50,
            UniTreeGroupId = 50,
            Amount = 400m,
            Reference = "RACE-TEST"
        };

        // ACT
        // Simulate two concurrent tasks.
        // the final state to be consistent.
        
        var task1 = _transactionsService.ProcessContribute(request);
        var task2 = _transactionsService.ProcessContribute(request);

        // ASSERT
        // Expecting either an exception due to concurrency or due to duplicate reference
        await Assert.ThrowsAnyAsync<Exception>(() => Task.WhenAll(task1, task2));

        _context.ChangeTracker.Clear();

        var updatedWallet = await _context.Wallets.FindAsync(50);

        // The balance should never be negative
        Assert.True(updatedWallet!.Balance >= 0m); 
        // With 500 balance and 400 contribution, only one should succeed
        Assert.Equal(100m, updatedWallet.Balance); 
    }

    public void Dispose()
    {
        _context?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        _serviceProvider?.Dispose();
    }
}