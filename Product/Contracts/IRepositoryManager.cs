namespace Product.Contracts;

public interface IRepositoryManager
{
    IProductRepository Product { get; }
    Task SaveAsync();
}