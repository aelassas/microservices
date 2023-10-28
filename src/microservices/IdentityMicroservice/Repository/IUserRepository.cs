using IdentityMicroservice.Model;

namespace IdentityMicroservice.Repository;

public interface IUserRepository
{
    User? GetUser(string email);
    void InsertUser(User user);
}