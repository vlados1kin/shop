using Microsoft.EntityFrameworkCore;
using Repository;
using Shared.Features;
using User.Contracts;
using User.DTO;

namespace User.Repository;

public class UserRepository : RepositoryBase<Entities.Models.User>, IUserRepository
{
    public UserRepository(RepositoryContext context) : base(context)
    {
    }
    
    public async Task<PagedList<Entities.Models.User>> GetUsersAsync(UserParameters userParameters, bool trackChanges)
    {
        var authors = await FindAll(trackChanges).ToListAsync();
        return PagedList<Entities.Models.User>.ToPagedList(authors, userParameters.PageNumber, userParameters.PageSize);
    }

    public async Task<Entities.Models.User> GetUserAsync(Guid id, bool trackChanges)
        => await FindByCondition(u => u.Id.Equals(id), trackChanges).SingleOrDefaultAsync();
}