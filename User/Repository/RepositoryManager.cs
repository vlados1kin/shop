using Repository;
using User.Contracts;

namespace User.Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IUserRepository> _userRepository;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _userRepository = new Lazy<IUserRepository>(() => new UserRepository(repositoryContext));
    }

    public IUserRepository User => _userRepository.Value;
    public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
}