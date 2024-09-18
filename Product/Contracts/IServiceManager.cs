namespace Product.Contracts;

public interface IServiceManager
{
    IProductService ProductService { get; }
}