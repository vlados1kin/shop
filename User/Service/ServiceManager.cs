using AutoMapper;
using Microsoft.AspNetCore.Identity;
using User.Contracts;

namespace User.Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IUserService> _userService;

    public ServiceManager(
        IRepositoryManager repositoryManager, 
        IMapper mapper,
        UserManager<Entities.Models.User> userManager, 
        IConfiguration configuration)
    {
        _userService = new Lazy<IUserService>(() => new UserService(repositoryManager, configuration, mapper, userManager));
    }

    public IUserService UserService => _userService.Value;
}