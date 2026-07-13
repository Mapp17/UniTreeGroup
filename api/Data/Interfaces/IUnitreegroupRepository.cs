public interface IUniTreeGroupRepository : IRepositoryWrapper<UniTreeGroup>
{
    Task<IEnumerable<UniTreeGroup>> GetAllWithDetailsAsync();
    Task<UniTreeGroup?> GetByIdWithDetailsAsync(int id);
    Task<bool> IsUserInAnyGroupAsync(int userId);
    Task<bool> AddMemberAsync(Membership membership);
}