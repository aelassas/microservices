using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CartMicroservice.Model;

public class CartItem
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string? CatalogItemId { get; init; }
    public required string CatalogItemName { get; set; }
    public decimal CatalogItemPrice { get; set; }
    public int Quantity { get; set; }
}