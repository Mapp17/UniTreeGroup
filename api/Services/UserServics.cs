public class UserServices
{
    private readonly IUnitOfWork _unitOfWork;
    public UserServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IEnumerable<UserDto> GetAllUsers()
    {
        var users = _unitOfWork.Users.Find(_ => true); // Using Find as a proxy for GetAll if GetAll isn't async here or just to show pattern
        return users.Select(user => new UserDto
        {
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        }).ToList();
    }
    
    public UserWalletDto GetUserWallet(int id)
    {
        var user = _unitOfWork.Users.GetByIdWithWallet(id);

        // GUARD CLAUSE: Check if user exists
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} was not found.");
        }

        // GUARD CLAUSE: Check if wallet is initialized
        if (user.Wallet == null)
        {
            return new UserWalletDto
            {
                UserId = user.Id,
                FullNames = user.FullName,
                Balance = 0m, // Default balance for uninitialized wallet
                Currency = "ZAR" // Default currency
            };
        }

        return new UserWalletDto
        {
            UserId = user.Id,
            FullNames = user.FullName,
            Balance = user.Wallet.Balance,
            Currency = user.Wallet.Currency
        };
    }

    
    

    public async Task<User> CreateNewUser(CreateDto newUserdto)
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
        var existingUser = _unitOfWork.Users.GetByEmail(newUserdto.Email);
        if (existingUser != null)
        {
            throw new ConflictException("A user with this email already exists.", new { Email = newUserdto.Email });
        }

        // Create user with an initialized wallet
        User newuser = new User
        {
            FullName = newUserdto.FullName,
            Email = newUserdto.Email,
            PhoneNumber = newUserdto.PhoneNumber,
            Wallet = new Wallet
            {
                Balance = 0m,
                Currency = "ZAR"
            }
        };

        await _unitOfWork.Users.AddAsync(newuser);
        await _unitOfWork.CompleteAsync();
        return newuser;
    }
}