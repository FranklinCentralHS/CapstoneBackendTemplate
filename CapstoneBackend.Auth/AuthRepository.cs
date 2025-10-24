using CapstoneBackend.Auth.Models;
using CapstoneBackend.Utilities;
using Dapper;
using Dapper.Contrib.Extensions;
using MySqlConnector;

namespace CapstoneBackend.Auth;

internal class AuthRepository : IAuthRepository
{
    public AuthRepository() {}
    
    async Task<ApiUser> IAuthRepository.Register(ApiUser user)
    {
        CryptographyUtility.CreatePasswordHash(user.Password, out var hash, out var salt);

        var dbUser = new DatabaseUser
        {
            CreateDatetime = user.CreateDatetime,
            Username = user.Username,
            EmailAddress = user.EmailAddress,
            PasswordHash = hash,
            PasswordSalt = salt,
            IsDeleted = false
        };
        
        var connectionString = Environment.GetEnvironmentVariable(EnvironmentVariables.MYSQL_CONNECTION_STRING);
        await using var connection = new MySqlConnection(connectionString);
        
        //make sure to return the api object, not the database object
        user.Id = await connection.InsertAsync(dbUser);

        return user;
    }

    async Task<DatabaseUser?> IAuthRepository.GetUserByUsername(Login credentials)
    {
        var query = "SELECT * FROM `Users` WHERE `Username` = @username";
        
        var connectionString = Environment.GetEnvironmentVariable(EnvironmentVariables.MYSQL_CONNECTION_STRING);
        await using var connection = new MySqlConnection(connectionString);

        return await connection.QuerySingleOrDefaultAsync<DatabaseUser>(query);
    }
}