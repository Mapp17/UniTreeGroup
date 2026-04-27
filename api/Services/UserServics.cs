public class UserServices
{
    private readonly UserRepository _userRepository;
    public UserServices(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public IEnumerable<UserDto> GetAllUsers()
    {
        var users = _userRepository.GetAllUsers();
        return users.Select(user => new UserDto
        {
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        }).ToList();
    }
    
    public UserWalletDto GetUserWallet(int id)
    {
        var user = _userRepository.GetUserById(id);

        // GUARD CLAUSE: Check if user exists
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }

        // GUARD CLAUSE: Check if wallet is initialized
        if (user.Wallet == null)
        {
            throw new BadRequestException("User does not have an active wallet.");
        }

        return new UserWalletDto
        {
            UserId = user.Id,
            FullNames = user.FullName,
            Balance = user.Wallet.Balance,
            Currency = user.Wallet.Currency
        };
    }

    public User CreateNewUser(CreateDto newUserdto)
    {
        // GUARD CLAUSE: Basic Validation
        if (string.IsNullOrWhiteSpace(newUserdto.Email))
        {
            var errors = new Dictionary<string, string[]> { 
                { "Email", new[] { "Email is required." } } 
            };
            throw new ValidationException(errors);
        }

        // GUARD CLAUSE: Check for existing email
        var existingUser = _userRepository.GetUserByEmail(newUserdto.Email);
        if (existingUser != null)
        {
            throw new ConflictException("A user with this email already exists.", new { Email = newUserdto.Email });
        }

        User newuser = new User
        {
            FullName = newUserdto.FullName,
            Email = newUserdto.Email,
            PhoneNumber = newUserdto.PhoneNumber
        };

        return _userRepository.CreateUser(newuser);
    }
}