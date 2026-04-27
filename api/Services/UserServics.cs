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


}