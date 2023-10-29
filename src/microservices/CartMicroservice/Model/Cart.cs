using MongoDB.Bson;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace CartMicroservice.Model;

public class Cart
{
    public static readonly string DocumentName = "carts";

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; init; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? UserId { get; init; }
    public List<CartItem> CartItems { get; init; } = new();
}