namespace CapstoneBackend.Auth.Models;

// This is the user data that should be used with the api.
public class ApiUser
{
    public int Id { get; set; }
    public DateTime CreateDatetime { get; set; }
    public string Username { get; set; } = "";
    public string EmailAddress { get; set; } = "";
    public string Password { get; set; } = "";
    public bool IsDeleted { get; set; }
}