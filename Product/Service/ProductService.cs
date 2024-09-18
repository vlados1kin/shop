using AutoMapper;
using Entities.Exceptions;
using Product.Contracts;
using Product.DTO;
using Shared.Features;

namespace Product.Service;

public class ProductService : IProductService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    
    public ProductService(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<(IEnumerable<ProductDto> users, MetaData metaData)> GetProductsAsync(Guid id, ProductParameters productParameters, bool trackChanges)
    {
        if (!productParameters.ValidPrice)
            throw new ValidPriceBadRequestException();
        var usersWithMetaData = await _repository.Product.GetProductsAsync(id, productParameters, trackChanges);
        var usersDto = _mapper.Map<IEnumerable<ProductDto>>(usersWithMetaData);
        return (users: usersDto, metaData: usersWithMetaData.MetaData);
    }

    public async Task<ProductDto> GetProductAsync(Guid id, Guid productId, bool trackChanges)
    {
        var product = await GetProductAndCheckIfItExists(id, productId, trackChanges);
        var productDto = _mapper.Map<ProductDto>(product);
        return productDto;
    }

    public async Task<ProductDto> CreateProductAsync(Guid id, ProductForCreationDto productForCreationDto)
    {
        var product = _mapper.Map<Entities.Models.Product>(productForCreationDto);
        product.UserId = id;
        _repository.Product.Create(product);
        await _repository.SaveAsync();
        var productDto = _mapper.Map<ProductDto>(product);
        return productDto;
    }

    public async Task UpdateProductAsync(Guid id, Guid productId, ProductForUpdateDto productForUpdateDto, bool trackChanges)
    {
        var product = await GetProductAndCheckIfItExists(id, productId, trackChanges);
        product.UserId = id;
        _mapper.Map(productForUpdateDto, product);
        await _repository.SaveAsync();
    }

    public async Task DeleteProductAsync(Guid id, Guid productId, bool trackChanges)
    {
        var product = await GetProductAndCheckIfItExists(id, productId, trackChanges);
        _repository.Product.Delete(product);
        await _repository.SaveAsync();
    }
    
    private async Task<Entities.Models.Product> GetProductAndCheckIfItExists(Guid id, Guid productId, bool trackChanges)
    {
        var product = await _repository.Product.GetProductAsync(id, productId, trackChanges);
        if (product is null)
            throw new ProductNotFoundException(id);
        return product;
    }
}