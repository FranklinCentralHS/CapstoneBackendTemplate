using Dapper;
using MySqlConnector;

namespace CapstoneBackend.Utilities;


public class DbConnectionTest
{
    public async Task<bool> TestConnection()
    {
        var connectionString = Environment.GetEnvironmentVariable(EnvironmentVariables.MYSQL_CONNECTION_STRING);
        await using var connection = new MySqlConnection(connectionString);

        var test = await connection.QueryFirstAsync<int>("SELECT 1;");

        return test == 1;
    }
}