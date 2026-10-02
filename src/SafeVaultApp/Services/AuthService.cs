using SafeVaultApp.Models;
using SafeVaultApp.Security;

namespace SafeVaultApp.Services;

public class AuthService
{
    private readonly PasswordHasher _passwordHasher;
    private readonly List<UserAccount> _users;
    private readonly Dictionary<string, AuthenticatedUser> _activeTokens = new();

    public AuthService(PasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;

        _users = new List<UserAccount>
        {
            new UserAccount
            {
                Username = "admin",
                PasswordHash = _passwordHasher.HashPassword("Admin!234"),
                Role = "Admin"
            },
            new UserAccount
            {
                Username = "user",
                PasswordHash = _passwordHasher.HashPassword("User!234"),
                Role = "User"
            }
        };
    }

    public AuthenticatedUser? Login(string? username, string? password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        UserAccount? user = _users.FirstOrDefault(account =>
            account.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase));

        if (user is null)
        {
            return null;
        }

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        var authenticatedUser = new AuthenticatedUser
        {
            Username = user.Username,
            Role = user.Role,
            Token = token
        };

        _activeTokens[token] = authenticatedUser;

        return authenticatedUser;
    }

    public AuthenticatedUser? ValidateToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        return _activeTokens.TryGetValue(token, out var user)
            ? user
            : null;
    }
}
