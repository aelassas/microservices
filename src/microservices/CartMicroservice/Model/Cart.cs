using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace CartMicroservice.Model;

public class Cart
{
    public static readonly string DocumentName = nameof(Cart);

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; init; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? UserId { get; init; }
    public List<CartItem> CartItems { get; init; } = new();
}