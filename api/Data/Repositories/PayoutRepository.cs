using Microsoft.EntityFrameworkCore;

public class PayoutRepository
{
    private readonly UniTreeDbContext _context;

    public PayoutRepository(UniTreeDbContext context)
    {
        _context = context;
    }

    public PayoutSchedule Create(PayoutSchedule payout)
    {
        _context.PayoutSchedules.Add(payout);
        _context.SaveChanges();
        return payout;
    }

    public IEnumerable<PayoutSchedule> GetByBeneficiaryId(int userId)
    {
        return _context.PayoutSchedules
            .Include(p => p.BeneficiaryUser)
            .Where(p => p.BeneficiaryUserId == userId)
            .ToList();
    }
}