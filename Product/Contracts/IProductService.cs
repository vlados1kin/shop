using Product.DTO;
using Shared.Features;

namespace Product.Contracts;

public interface IProductService
{
    Task<(IEnumerable<ProductDto> users, MetaData metaData)> GetProductsAsync(Guid id, ProductParameters productParameters, bool trackChanges);
    Task<ProductDto> GetProductAsync(Guid id, Guid productId, bool trackChanges);
    Task<ProductDto> CreateProductAsync(Guid id, ProductForCreationDto productForCreationDto);
    Task UpdateProductAsync(Guid id, Guid productId, ProductForUpdateDto productForUpdateDto, bool trackChanges);
    Task DeleteProductAsync(Guid id, Guid productId, bool trackChanges);
}