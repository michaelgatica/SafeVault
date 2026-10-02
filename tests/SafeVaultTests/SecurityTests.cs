using SafeVaultApp.Data;
using SafeVaultApp.Models;
using SafeVaultApp.Security;
using SafeVaultApp.Validation;
using Xunit;

namespace SafeVaultTests;

public class SecurityTests
{
    [Fact]
    public void PasswordHasher_VerifiesValidPassword_AndRejectsInvalidPassword()
    {
        var hasher = new PasswordHasher();

        string hash = hasher.HashPassword("Secure!234");

        Assert.True(hasher.VerifyPassword("Secure!234", hash));
        Assert.False(hasher.VerifyPassword("WrongPassword", hash));
    }

    [Fact]
    public void InputValidator_RejectsSqlInjectionPattern()
    {
        var validator = new InputValidator();

        var request = new VaultRecordRequest
        {
            Title = "' OR 1=1 --",
            SecretValue = "test",
            Notes = "test"
        };

        var errors = validator.ValidateVaultRecord(request);

        Assert.True(errors.ContainsKey("Title"));
    }

    [Fact]
    public void HtmlSanitizer_EncodesScriptTags()
    {
        var sanitizer = new HtmlSanitizer();

        string encoded = sanitizer.Encode("<script>alert('xss')</script>");

        Assert.DoesNotContain("<script>", encoded);
        Assert.Contains("&lt;script&gt;", encoded);
    }

    [Fact]
    public void AuthorizationGuard_AllowsAdminRole_AndRejectsStandardUserForAdmin()
    {
        var admin = new AuthenticatedUser
        {
            Username = "admin",
            Role = "Admin",
            Token = "token"
        };

        var user = new AuthenticatedUser
        {
            Username = "user",
            Role = "User",
            Token = "token"
        };

        Assert.True(AuthorizationGuard.HasAnyRole(admin, "Admin"));
        Assert.False(AuthorizationGuard.HasAnyRole(user, "Admin"));
        Assert.True(AuthorizationGuard.HasAnyRole(user, "Admin", "User"));
    }

    [Fact]
    public async Task Repository_ParameterizedSearch_DoesNotReturnAllRecordsForInjectionInput()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");
        string connectionString = $"Data Source={databasePath}";

        try
        {
            var repository = new SafeVaultRepository(connectionString);
            await repository.InitializeAsync();

            await repository.AddRecordAsync(new VaultRecord
            {
                Title = "Confidential Record",
                SecretValue = "Secret",
                Notes = "Normal note",
                CreatedBy = "tester",
                CreatedAt = DateTime.UtcNow
            });

            IReadOnlyList<VaultRecord> results = await repository.SearchRecordsAsync("' OR 1=1 --");

            Assert.Empty(results);
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }
}
