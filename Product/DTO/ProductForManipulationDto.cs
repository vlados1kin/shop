namespace Product.DTO;

public abstract record ProductForManipulationDto
{
    public string Name { get; set; }
    public string Title { get; set; }
    public double Price { get; set; }
    public bool IsVisible { get; set; }
}