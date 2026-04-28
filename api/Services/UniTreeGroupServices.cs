public class UniTreeGroupServices
{
    private readonly UniTreeGroupRepository _repo;
    private readonly UserRepository _userRepo;

    public UniTreeGroupServices(UniTreeGroupRepository repo, UserRepository userRepo)
    {
        _repo = repo;
        _userRepo = userRepo;
    }

    public GroupReadDto CreateGroup(CreateGroupDto dto)
    {
        var existingGroups = _repo.GetAll();
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

        var creator = _userRepo.GetUserById(dto.CreatedById);
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

        var created = _repo.Create(group);
        return MapToDto(created);
    }

    public async Task<bool> JoinGroupAsync(JoinGroupDto dto)
    {
        var group = await _repo.GetByIdAsync(dto.UniTreeGroupId);
        if (group == null) 
            throw new NotFoundException("Group not found.");

        if (group.Memberships.Count >= group.MaxMembers)
            throw new BadRequestException("This group has reached its maximum capacity.");

        // UPDATED: Use the corrected check for any group membership
        var alreadyMember = await _repo.IsUserInAnyGroupAsync(dto.UserId);
        if (alreadyMember) 
            throw new ConflictException("User is already a member of a group (Users can only join one group).");

        var membership = new Membership
        {
            UserId = dto.UserId,
            UniTreeGroupId = dto.UniTreeGroupId, // Ensure this is set
            JoinedAt = DateTime.UtcNow,
            Status = MembershipStatus.Active, 
            PayoutOrder = group.Memberships.Count + 1, 
            TotalContributed = 0m
        };

        return await _repo.AddMemberAsync(membership);
    }

    public IEnumerable<GroupMemberDto> GetMembers(int groupId)
    {
        var group = _repo.GetById(groupId);
        if (group == null) throw new NotFoundException("Group not found.");

        return group.Memberships.Select(m => new GroupMemberDto
        {
            UserId = m.UserId,
            FullName = m.User.FullName,
            Email = m.User.Email,
            JoinedAt = m.JoinedAt
        });
    }

    public IEnumerable<GroupReadDto> GetAllGroups() => _repo.GetAll().Select(MapToDto);

    public GroupReadDto GetGroupById(int id)
    {
        var group = _repo.GetById(id);
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