using System;
using System.Security.Cryptography;

namespace Middleware;

public class Encryptor : IEncryptor
{
    private const int SALT_SIZE = 40;
    private const int ITERATIONS_COUNT = 10000;

    public string GetSalt()
    {
        var saltBytes = new byte[SALT_SIZE];
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);

        return Convert.ToBase64String(saltBytes);
    }

    public string GetHash(string value, string salt)
    {
        var pbkdf2 = new Rfc2898DeriveBytes(value, GetBytes(salt), ITERATIONS_COUNT, HashAlgorithmName.SHA256);

        return Convert.ToBase64String(pbkdf2.GetBytes(SALT_SIZE));
    }

    private static byte[] GetBytes(string value)
    {
        var bytes = new byte[value.Length + sizeof(char)];
        Buffer.BlockCopy(value.ToCharArray(), 0, bytes, 0, bytes.Length);

        return bytes;
    }
}