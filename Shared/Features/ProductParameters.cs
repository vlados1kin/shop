namespace Shared.Features;

public class ProductParameters : QueryParameters
{
    public double MinPrice { get; set; }
    public double MaxPrice { get; set; } = int.MaxValue;
    
    public bool ValidPrice => MaxPrice > MinPrice;
    public string? Search { get; set; }
}