using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Contracts;
using Product.DTO;
using Shared.Features;
using Shared.Filters;

namespace Product.Presentation;

[Route("api/users/{id:guid}/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IServiceManager _service;

    public ProductController(IServiceManager service) => _service = service;

    [HttpHead]
    [HttpGet(Name = "GetProducts")]
    //[Authorize(Policy = "AdminAndSelfOnly")]
    public async Task<IActionResult> GetProducts([FromRoute] Guid id, [FromQuery] ProductParameters productParameters)
    {
        var pagedResult = await _service.ProductService.GetProductsAsync(id, productParameters, trackChanges: false);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
        return Ok(pagedResult.users);
    }

    [HttpGet("{productId:guid}", Name = "GetProductById")]
    //[Authorize(Policy = "AdminAndSelfOnly")]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id, [FromRoute] Guid productId)
    {
        var product = await _service.ProductService.GetProductAsync(id, productId, false);
        return Ok(product);
    }

    [HttpPost(Name = "CreateProducts")]
    //[Authorize(Policy = "AdminAndSelfOnly")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateProduct([FromRoute] Guid id,
        [FromBody] ProductForCreationDto productForCreationDto)
    {
        var createdProduct = await _service.ProductService.CreateProductAsync(id, productForCreationDto);
        return CreatedAtRoute("GetProductById", new { id, productId = createdProduct.Id }, createdProduct);
    }

    [HttpPut("{productId:guid}")]
    //[Authorize(Policy = "AdminAndSelfOnly")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromRoute] Guid productId,
        [FromBody] ProductForUpdateDto productForUpdateDto)
    {
        await _service.ProductService.UpdateProductAsync(id, productId, productForUpdateDto, true);
        return NoContent();
    }

    [HttpDelete("{productId:guid}")]
    //[Authorize(Policy = "AdminAndSelfOnly")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, [FromRoute] Guid productId)
    {
        await _service.ProductService.DeleteProductAsync(id, productId, false);
        return NoContent();
    }
}