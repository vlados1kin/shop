using Microsoft.EntityFrameworkCore;
using Product.Contracts;
using Repository;
using Shared.Features;

namespace Product.Repository;

public class ProductRepository : RepositoryBase<Entities.Models.Product>, IProductRepository
{
    public ProductRepository(RepositoryContext context) : base(context)
    {
    }

    public async Task<PagedList<Entities.Models.Product>> GetProductsAsync(Guid id, ProductParameters productParameters,
        bool trackChanges)
    {
        var products = await FindByCondition(a => a.UserId == id, trackChanges).ToListAsync();
        return PagedList<Entities.Models.Product>.ToPagedList(products, productParameters.PageNumber,
            productParameters.PageSize);
    }

    public async Task<Entities.Models.Product> GetProductAsync(Guid id, Guid productId, bool trackChanges)
        => await FindByCondition(c => c.UserId.Equals(id) && c.Id.Equals(productId), trackChanges)
            .SingleOrDefaultAsync()!;
}