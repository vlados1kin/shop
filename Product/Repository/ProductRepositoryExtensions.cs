namespace Product.Repository;

public static class ProductRepositoryExtensions
{
    public static IQueryable<Entities.Models.Product> Filter(this IQueryable<Entities.Models.Product> products,
        double minPrice, double maxPrice)
        => products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);

    public static IQueryable<Entities.Models.Product> Search(this IQueryable<Entities.Models.Product> products, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return products;
        var lowerCaseTerm = searchTerm.Trim().ToLower();
        return products.Where(p => p.Name.ToLower().Contains(lowerCaseTerm));
    }
}