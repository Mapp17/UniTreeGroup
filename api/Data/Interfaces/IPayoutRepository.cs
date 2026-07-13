public interface IPayoutRepository : IRepositoryWrapper<PayoutSchedule>
{
    IEnumerable<PayoutSchedule> GetByBeneficiaryId(int userId);
}
