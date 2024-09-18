namespace Entities.Exceptions;

public sealed class ProductNotFoundException : NotFoundException
{
    public ProductNotFoundException(Guid id) : base($"The product with id: {id} does not exist in the database.")
    {
    }
}