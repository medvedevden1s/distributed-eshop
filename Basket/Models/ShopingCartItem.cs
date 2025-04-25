namespace Basket.Models;

public class ShoppingCartItem
{
    public int    Quantity    { get; set; } = default!;
    public string Color       { get; set; } = default!;
    public int    ProductId   { get; set; } = default!;  // ID from Catalog module
    public decimal Price      { get; set; } = default!;  // Unit price from Catalog
    public string ProductName { get; set; } = default!;  // Name from Catalog
}
