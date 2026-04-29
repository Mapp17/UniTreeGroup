public class PayoutServices
{
    private readonly IUnitOfWork _unitOfWork;

    public PayoutServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PayoutReadDto> SchedulePayoutAsync(CreatePayoutDto dto)
    {
        // GUARD: Ensure Amount is positive
        if (dto.Amount <= 0)
            throw new BadRequestException("Payout amount must be greater than zero.");

        // GUARD: Ensure Date is in the future
        if (dto.ScheduledDate < DateTime.UtcNow)
            throw new BadRequestException("Scheduled date cannot be in the past.");

        // GUARD: Ensure Beneficiary exists
        var user = _unitOfWork.Users.GetByIdWithWallet(dto.BeneficiaryUserId);
        if (user == null)
            throw new NotFoundException($"Beneficiary user with ID {dto.BeneficiaryUserId} does not exist.");

        var payout = new PayoutSchedule
        {
            BeneficiaryUserId = dto.BeneficiaryUserId,
            Amount = dto.Amount,
            ScheduledDate = dto.ScheduledDate,
            Status = PayoutScheduleStatus.Scheduled
        };

        await _unitOfWork.Payouts.AddAsync(payout);
        await _unitOfWork.CompleteAsync();

        return MapToDto(payout);
    }

    public async Task<IEnumerable<PayoutReadDto>> GetPayoutGroupAsync(int beneficiaryId)
    {
        var payouts = _unitOfWork.Payouts.GetByBeneficiaryId(beneficiaryId);
        
        // GUARD: Return empty list or throw if the "group" (user) doesn't exist
        if (!payouts.Any() && _unitOfWork.Users.GetByIdWithWallet(beneficiaryId) == null)
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