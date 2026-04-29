using Microsoft.EntityFrameworkCore;

public interface IPayoutRepository : IGenericRepository<PayoutSchedule>
{
    IEnumerable<PayoutSchedule> GetByBeneficiaryId(int userId);
}

public class PayoutRepository : GenericRepository<PayoutSchedule>, IPayoutRepository
{
    public PayoutRepository(UniTreeDbContext context) : base(context) { }

    public IEnumerable<PayoutSchedule> GetByBeneficiaryId(int userId)
    {
        return _context.PayoutSchedules
            .Include(p => p.BeneficiaryUser)
            .Where(p => p.BeneficiaryUserId == userId)
            .ToList();
    }
}