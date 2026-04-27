public class PayoutServices
{
    private readonly PayoutRepository _payoutRepository;
    private readonly UserRepository _userRepository;

    public PayoutServices(PayoutRepository payoutRepository, UserRepository userRepository)
    {
        _payoutRepository = payoutRepository;
        _userRepository = userRepository;
    }

    public PayoutReadDto SchedulePayout(CreatePayoutDto dto)
    {
        // GUARD: Ensure Amount is positive
        if (dto.Amount <= 0)
            throw new BadRequestException("Payout amount must be greater than zero.");

        // GUARD: Ensure Date is in the future
        if (dto.ScheduledDate < DateTime.UtcNow)
            throw new BadRequestException("Scheduled date cannot be in the past.");

        // GUARD: Ensure Beneficiary exists
        var user = _userRepository.GetUserById(dto.BeneficiaryUserId);
        if (user == null)
            throw new NotFoundException($"Beneficiary user with ID {dto.BeneficiaryUserId} does not exist.");

        var payout = new PayoutSchedule
        {
            BeneficiaryUserId = dto.BeneficiaryUserId,
            Amount = dto.Amount,
            ScheduledDate = dto.ScheduledDate,
            Status = PayoutScheduleStatus.Scheduled
        };

        var created = _payoutRepository.Create(payout);

        return MapToDto(created);
    }

    public IEnumerable<PayoutReadDto> GetPayoutGroup(int beneficiaryId)
    {
        var payouts = _payoutRepository.GetByBeneficiaryId(beneficiaryId);
        
        // GUARD: Return empty list or throw if the "group" (user) doesn't exist
        if (!payouts.Any() && _userRepository.GetUserById(beneficiaryId) == null)
            throw new NotFoundException("No payouts found for the specified user group.");

        return payouts.Select(MapToDto);
    }

    private static PayoutReadDto MapToDto(PayoutSchedule p) => new PayoutReadDto
    {
        Id = p.Id,
        BeneficiaryUserId = p.BeneficiaryUserId,
        BeneficiaryName = p.BeneficiaryUser?.FullName ?? "Unknown",
        Amount = p.Amount,
        ScheduledDate = p.ScheduledDate,
        Status = p.Status.ToString()
    };
}