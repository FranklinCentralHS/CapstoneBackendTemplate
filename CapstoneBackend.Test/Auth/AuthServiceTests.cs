using CapstoneBackend.Auth;
using CapstoneBackend.Auth.Models;
using CapstoneBackend.Utilities;
using CapstoneBackend.Utilities.Exceptions;
using Moq;

namespace CapstoneBackend.Test.Auth;

public class AuthServiceTests
{
    [Fact]
    public async Task Login_WhenCalledWithoutUsername_ThrowsBadRequest()
    {
        var repo = new Mock<IAuthRepository>();
        
        var service = new AuthService(repo.Object);

        var badLogin = new Login
        {
            Username = "",
            Password = "it's a secret"
        };

        var test = async () => await service.Login(badLogin);

        await Assert.ThrowsAsync<BadRequestException>(test);
    }
    
    [Fact]
    public async Task Login_WhenCalledWithoutPassword_ThrowsBadRequest()
    {
        var repo = new Mock<IAuthRepository>();
        
        var service = new AuthService(repo.Object);

        var badLogin = new Login
        {
            Username = "myUsername",
            Password = ""
        };

        var test = async () => await service.Login(badLogin);

        await Assert.ThrowsAsync<BadRequestException>(test);
    }
    
    [Fact]
    public async Task Login_UserDoesntExist_ThrowsUnauthenticated()
    {
        var repo = new Mock<IAuthRepository>();
        repo.Setup(r => r.GetUserByUsername(It.IsAny<Login>()))
            .ReturnsAsync((DatabaseUser) null);
        
        var service = new AuthService(repo.Object);

        var badLogin = new Login
        {
            Username = "myUsername",
            Password = "it's a secret"
        };

        var test = async () => await service.Login(badLogin);

        await Assert.ThrowsAsync<UnauthenticatedException>(test);
    }
    
    [Fact]
    public async Task Login_UserIsDeleted_ThrowsUnauthenticated()
    {
        var repo = new Mock<IAuthRepository>();
        repo.Setup(r => r.GetUserByUsername(It.IsAny<Login>()))
            .ReturnsAsync(new DatabaseUser
            {
                IsDeleted = true
            });
        
        var service = new AuthService(repo.Object);

        var badLogin = new Login
        {
            Username = "myUsername",
            Password = "it's a secret"
        };

        var test = async () => await service.Login(badLogin);

        await Assert.ThrowsAsync<UnauthenticatedException>(test);
    }
    
    [Fact]
    public async Task Login_UserPasswordIsWrong_ThrowsUnauthenticated()
    {
        var repo = new Mock<IAuthRepository>();
        repo.Setup(r => r.GetUserByUsername(It.IsAny<Login>()))
            .ReturnsAsync(new DatabaseUser
            {
                IsDeleted = false,
                PasswordHash = [1],
                PasswordSalt = [1]
            });
        
        var service = new AuthService(repo.Object);

        var badLogin = new Login
        {
            Username = "myUsername",
            Password = "it's a secret"
        };

        var test = async () => await service.Login(badLogin);

        await Assert.ThrowsAsync<UnauthenticatedException>(test);
    }
    
    [Fact]
    public async Task Login_IsSuccessful_ReturnsAuthToken()
    {
        var password = "it's a secret";
        
        Environment.SetEnvironmentVariable(EnvironmentVariables.TOKEN_KEY, "256 bits or more in order to be a valid key");
            
        var repo = new Mock<IAuthRepository>();
        CryptographyUtility.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
        repo.Setup(r => r.GetUserByUsername(It.IsAny<Login>()))
            .ReturnsAsync(new DatabaseUser
            {
                IsDeleted = false,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });
        
        var service = new AuthService(repo.Object);

        var badLogin = new Login
        {
            Username = "myUsername",
            Password = password
        };

        var test = await service.Login(badLogin);

        Assert.IsType<AuthToken>(test);
    }
}