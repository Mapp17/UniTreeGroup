using Microsoft.EntityFrameworkCore;

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
            // Reset sequence for PostgreSQL
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"Users\"', 'Id'), (SELECT MAX(\"Id\") FROM \"Users\"));");
        }

        if (!context.UniTreeGroups.Any())
        {
            context.UniTreeGroups.AddRange(
                new UniTreeGroup { Id = 1, Name = "Soweto Savings Circle", Description = "Monthly savings", CreatedById = 1, ContributionAmount = 500m, PayoutCycle = PayoutCycleType.Monthly, MaxMembers = 10 },
                new UniTreeGroup { Id = 2, Name = "Durban Festive Fund", Description = "Saving up", CreatedById = 4, ContributionAmount = 1000m, PayoutCycle = PayoutCycleType.Monthly, MaxMembers = 5 }
            );
            await context.SaveChangesAsync();
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"UniTreeGroups\"', 'Id'), (SELECT MAX(\"Id\") FROM \"UniTreeGroups\"));");
        }

        if (!context.Memberships.Any())
        {
            context.Memberships.AddRange(
                new Membership { Id = 1, UserId = 1, UniTreeGroupId = 1, PayoutOrder = 1 },
                new Membership { Id = 2, UserId = 2, UniTreeGroupId = 1, PayoutOrder = 2 },
                new Membership { Id = 3, UserId = 3, UniTreeGroupId = 1, PayoutOrder = 3 },
                new Membership { Id = 4, UserId = 4, UniTreeGroupId = 2, PayoutOrder = 1 },
                new Membership { Id = 5, UserId = 5, UniTreeGroupId = 2, PayoutOrder = 2 }
            );
            await context.SaveChangesAsync();
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"Memberships\"', 'Id'), (SELECT MAX(\"Id\") FROM \"Memberships\"));");
        }
    }
}