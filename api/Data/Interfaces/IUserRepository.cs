public interface IUserRepository : IRepositoryWrapper<User>
{
    User? GetByEmail(string email);
    User? GetByIdWithWallet(int id);
}