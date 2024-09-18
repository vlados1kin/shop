namespace Product.DTO;

public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Title { get; init; }
    public double Price { get; init; }
    public bool IsVisible { get; init; }
    public DateTime CreationTime { get; init; }
    public Guid UserId { get; init; }
}
