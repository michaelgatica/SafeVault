namespace SafeVaultApp.Models;

public class VaultRecord
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SecretValue { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
