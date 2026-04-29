public class UniTreeGroupServices
{
    private readonly IUnitOfWork _unitOfWork;

    public UniTreeGroupServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GroupReadDto> CreateGroup(CreateGroupDto dto)
    {
        var existingGroups = await _unitOfWork.Groups.GetAllAsync();
        if (existingGroups.Any(g => g.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ConflictException("A group with this name already exists.", new { Name = dto.Name });
        }
        
        if (dto.ContributionAmount <= 0)
            throw new BadRequestException("Contribution amount must be positive.");

        if (dto.MaxMembers < 2)
            throw new BadRequestException("A group must allow at least 2 members.");

        if (dto.CreatedById <= 0)
        {
            throw new BadRequestException("A valid User ID is required to create a group.");
        }

        var creator = _unitOfWork.Users.GetByIdWithWallet(dto.CreatedById);
        if (creator == null)
            throw new NotFoundException($"User {dto.CreatedById} not found.");

        var group = new UniTreeGroup
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedById = dto.CreatedById,
            ContributionAmount = dto.ContributionAmount,
            PayoutCycle = (PayoutCycleType)dto.PayoutCycle,
            MaxMembers = dto.MaxMembers
        };

        await _unitOfWork.Groups.AddAsync(group);
        await _unitOfWork.CompleteAsync();
        
        return MapToDto(group);
    }

    public async Task<bool> JoinGroupAsync(JoinGroupDto dto)
    {
        var group = await _unitOfWork.Groups.GetByIdAsync(dto.UniTreeGroupId);
        if (group == null) 
            throw new NotFoundException("Group not found.");

        if (group.Memberships.Count >= group.MaxMembers)
            throw new BadRequestException("This group has reached its maximum capacity.");

        var alreadyMember = await _unitOfWork.Groups.IsUserInAnyGroupAsync(dto.UserId);
        if (alreadyMember) 
            throw new ConflictException("User is already a member of a group (Users can only join one group).");

        var membership = new Membership
        {
            UserId = dto.UserId,
            UniTreeGroupId = dto.UniTreeGroupId,
            JoinedAt = DateTime.UtcNow,
            Status = MembershipStatus.Active, 
            PayoutOrder = group.Memberships.Count + 1, 
            TotalContributed = 0m
        };

        await _unitOfWork.Groups.AddMemberAsync(membership);
        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<IEnumerable<GroupMemberDto>> GetMembers(int groupId)
    {
        var group = await _unitOfWork.Groups.GetByIdWithDetailsAsync(groupId);
        if (group == null) throw new NotFoundException("Group not found.");

        return group.Memberships.Select(m => new GroupMemberDto
        {
            UserId = m.UserId,
            FullName = m.User.FullName,
            Email = m.User.Email,
            JoinedAt = m.JoinedAt
        });
    }

    public async Task<IEnumerable<GroupReadDto>> GetAllGroups() 
    {
        var groups = await _unitOfWork.Groups.GetAllWithDetailsAsync();
        return groups.Select(MapToDto);
    }

    public async Task<GroupReadDto> GetGroupById(int id)
    {
        var group = await _unitOfWork.Groups.GetByIdWithDetailsAsync(id);
        if (group == null) throw new NotFoundException("Group not found.");
        return MapToDto(group);
    }

    private GroupReadDto MapToDto(UniTreeGroup g) => new GroupReadDto
    {
        Id = g.Id,
        Name = g.Name,
        Description = g.Description,
        CreatedByUserName = g.CreatedBy?.FullName ?? "Unknown",
        ContributionAmount = g.ContributionAmount,
        PayoutCycle = g.PayoutCycle.ToString(),
        MaxMembers = g.MaxMembers,
        CurrentMemberCount = g.Memberships?.Count ?? 0,
        Status = g.Status.ToString(),
        PoolBalance = g.PoolBalance
    };
}