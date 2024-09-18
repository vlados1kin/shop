using Contracts;
using Shared.Features;
using User.DTO;

namespace User.Contracts;

public interface IUserRepository : IRepositoryBase<Entities.Models.User>
{
    Task<PagedList<Entities.Models.User>> GetUsersAsync(UserParameters userParameters, bool trackChanges);
    Task<Entities.Models.User> GetUserAsync(Guid id, bool trackChanges);
}