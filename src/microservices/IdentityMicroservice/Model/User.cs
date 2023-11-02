using Middleware;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityMicroservice.Model;

public class User
{
    public static readonly string DocumentName = nameof(User);

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; init; }
    public required string Email { get; init; }
    public required string Password { get; set; }
    public string? Salt { get; set; }
    public bool IsAdmin { get; init; }

    public void SetPassword(string password, IEncryptor encryptor)
    {
        Salt = encryptor.GetSalt();
        Password = encryptor.GetHash(password, Salt);
    }

    public bool ValidatePassword(string password, IEncryptor encryptor) =>
        Password == encryptor.GetHash(password, Salt);
}