using CapstoneBackend.Auth.Models;

namespace CapstoneBackend.Auth;

public interface IAuthRepository
{
    internal Task<ApiUser> Register(ApiUser databaseUser);
    internal Task<DatabaseUser?> GetUserByUsername(Login credentials);
}