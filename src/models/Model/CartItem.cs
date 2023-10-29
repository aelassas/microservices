namespace Model;

public class CartItem
{
    public CatalogItem? CatalogItem { get; init; }
    public int Quantity { get; set; }
}