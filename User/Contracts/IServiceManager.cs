namespace User.Contracts;

public interface IServiceManager
{
    IUserService UserService { get; }
}