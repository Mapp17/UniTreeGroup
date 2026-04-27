

public static class AppSeeder
{
    public static async Task SeedAsync(UniTreeDbContext context)
    {

        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User { Id = 1, FullName = "Thabo Mokoena", Email = "thabo.mokoena@gmail.com", PhoneNumber = "0823456789", PasswordHash = "hashed_pw_1" },
                new User { Id = 2, FullName = "Lerato Nkosi", Email = "lerato.nkosi@gmail.com", PhoneNumber = "0734567890", PasswordHash = "hashed_pw_2" },
                new User { Id = 3, FullName = "Sipho Dlamini", Email = "sipho.dlamini@gmail.com", PhoneNumber = "0712345678", PasswordHash = "hashed_pw_3" },
                new User { Id = 4, FullName = "Nomsa Khumalo", Email = "nomsa.k@gmail.com", PhoneNumber = "0845671234", PasswordHash = "hashed_pw_4" },
                new User { Id = 5, FullName = "Kabelo Ndlovu", Email = "kabelo.ndlovu@gmail.com", PhoneNumber = "0769876543", PasswordHash = "hashed_pw_5" }
            );

            await context.SaveChangesAsync();
        }

        if (!context.Wallets.Any())
        {
            context.Wallets.AddRange(
                new Wallet { Id = 1, UserId = 1, Balance = 1500m },
                new Wallet { Id = 2, UserId = 2, Balance = 2200m },
                new Wallet { Id = 3, UserId = 3, Balance = 800m },
                new Wallet { Id = 4, UserId = 4, Balance = 1200m },
                new Wallet { Id = 5, UserId = 5, Balance = 3000m }
            );

            await context.SaveChangesAsync();
        }


        if (!context.Memberships.Any())
        {
            context.Memberships.AddRange(
                new Membership { Id = 1, UserId = 1, PayoutOrder = 1, TotalContributed = 1500m },
                new Membership { Id = 2, UserId = 2, PayoutOrder = 2, TotalContributed = 1500m },
                new Membership { Id = 3, UserId = 3, PayoutOrder = 3, TotalContributed = 1500m },
                new Membership { Id = 4, UserId = 4, PayoutOrder = 1, TotalContributed = 2000m },
                new Membership { Id = 5, UserId = 5, PayoutOrder = 2, TotalContributed = 2000m }
            );

            await context.SaveChangesAsync();
        }


        if (!context.Transactions.Any())
        {
            context.Transactions.AddRange(
                new Transactions
                {
                    Id = 1,
                    UserId = 1,
                    Type = TransactionType.Deposit,
                    Status = TransactionStatus.Completed,
                    Amount = 2000m,
                    Reference = "DEP-001",
                    Description = "Initial funding"
                },
                new Transactions
                {
                    Id = 2,
                    UserId = 1,
                    Type = TransactionType.Contribution,
                    Status = TransactionStatus.Completed,
                    Amount = 500m,
                    Reference = "CONTR-001"
                },
                new Transactions
                {
                    Id = 3,
                    UserId = 2,
                    Type = TransactionType.Withdrawal,
                    Status = TransactionStatus.Completed,
                    Amount = 300m,
                    Reference = "WDR-001"
                },
                new Transactions
                {
                    Id = 4,
                    UserId = 3,
                    Type = TransactionType.Contribution,
                    Status = TransactionStatus.Failed,
                    Amount = 500m,
                    Reference = "FAIL-001",
                    Description = "Insufficient funds"
                }
            );

            await context.SaveChangesAsync();
        }

   
        if (!context.PayoutSchedules.Any())
        {
            context.PayoutSchedules.AddRange(
                new PayoutSchedule
                {
                    Id = 1,
                    BeneficiaryUserId = 1,
                    Amount = 1500m,
                    ScheduledDate = DateTime.UtcNow.AddDays(5)
                },
                new PayoutSchedule
                {
                    Id = 2,
                    BeneficiaryUserId = 2,
                    Amount = 1500m,
                    ScheduledDate = DateTime.UtcNow.AddMonths(1)
                },
                new PayoutSchedule
                {
                    Id = 3,
                    BeneficiaryUserId = 4,
                    Amount = 2000m,
                    ScheduledDate = DateTime.UtcNow.AddDays(10)
                }
            );

            await context.SaveChangesAsync();
        }
    }
}