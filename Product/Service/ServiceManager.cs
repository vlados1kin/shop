using AutoMapper;
using Product.Contracts;

namespace Product.Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IProductService> _productService;

    public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper)
    {
        _productService = new Lazy<IProductService>(() => new ProductService(repositoryManager, mapper));
    }

    public IProductService ProductService => _productService.Value;
}