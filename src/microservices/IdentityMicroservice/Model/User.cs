using Middleware;
using System;

namespace IdentityMicroservice.Model;

public class User
{
    public static readonly string DocumentName = "users";

    public Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Password { get; set; }
    public string? Salt { get; set; }
    public bool IsAdmin { get; init; }

    public void SetPassword(string password, IEncryptor encryptor)
    {
        Salt = encryptor.GetSalt();
        Password = encryptor.GetHash(password, Salt);
    }

    public bool ValidatePassword(string password, IEncryptor encryptor)
    {
        var isValid = Password.Equals(encryptor.GetHash(password, Salt));
        return isValid;
    }
}