using IdentityMicroservice.Model;
using MongoDB.Driver;

namespace IdentityMicroservice.Repository;

public class UserRepository : IUserRepository
{
    private readonly IMongoDatabase _db;

    public UserRepository(IMongoDatabase db)
    {
        _db = db;
    }

    public User? GetUser(string email)
    {
        var col = _db.GetCollection<User>(User.DocumentName);
        var user = col.Find(u => u.Email == email).FirstOrDefault();
        return user;
    }

    public void InsertUser(User user)
    {
        var col = _db.GetCollection<User>(User.DocumentName);
        col.InsertOne(user);
    }
}