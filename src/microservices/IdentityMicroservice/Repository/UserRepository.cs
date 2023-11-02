using IdentityMicroservice.Model;
using MongoDB.Driver;

namespace IdentityMicroservice.Repository;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _col;

    public UserRepository(IMongoDatabase db)
    {
        _col = db.GetCollection<User>(User.DocumentName);
    }

    public User? GetUser(string email) =>
        _col.Find(u => u.Email == email).FirstOrDefault();

    public void InsertUser(User user) =>
        _col.InsertOne(user);
}