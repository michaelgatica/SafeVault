namespace SafeVaultApp.Models;

public class VaultRecordRequest
{
    public string? Title { get; set; }
    public string? SecretValue { get; set; }
    public string? Notes { get; set; }
}
