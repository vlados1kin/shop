namespace User.Contracts;

public interface IRepositoryManager
{
    IUserRepository User { get; }
    Task SaveAsync();
}