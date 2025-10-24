using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CapstoneBackend.Auth.Models;
using CapstoneBackend.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace CapstoneBackend.Auth;

internal static class ClaimUtility
{
    private static readonly byte[] Key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable(EnvironmentVariables.TOKEN_KEY)!);
    
    internal static string CreateToken(DatabaseUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(CreateClaims(user)),
            Expires = DateTime.Now.AddDays(1), // set how long until the user will need to login again
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static IEnumerable<Claim> CreateClaims(DatabaseUser user)
    {
        //making it a list in case I need to add roles or permissions later
        var result = new List<Claim> {
            new (ClaimTypes.Name, user.Id.ToString()),
            new (ClaimTypes.Email, user.EmailAddress)
        };
            
        return result.ToArray();
    }
}