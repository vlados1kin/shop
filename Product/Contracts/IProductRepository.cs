using Contracts;
using Shared.Features;

namespace Product.Contracts;

public interface IProductRepository : IRepositoryBase<Entities.Models.Product>
{
    Task<PagedList<Entities.Models.Product>> GetProductsAsync(Guid id, ProductParameters productParameters, bool trackChanges);
    Task<Entities.Models.Product> GetProductAsync(Guid id, Guid productId, bool trackChanges);
}