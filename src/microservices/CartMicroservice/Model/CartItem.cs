using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CartMicroservice.Model;

public class CartItem
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string? CatalogItemId { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}