using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CatalogMicroservice.Model;

public class CatalogItem
{
    public static readonly string DocumentName = nameof(CatalogItem);

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}