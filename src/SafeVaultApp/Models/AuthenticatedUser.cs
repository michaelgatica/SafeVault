namespace SafeVaultApp.Models;

public class AuthenticatedUser
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string Token { get; set; } = string.Empty;
}
