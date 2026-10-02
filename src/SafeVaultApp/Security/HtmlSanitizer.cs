using System.Text.Encodings.Web;

namespace SafeVaultApp.Security;

public class HtmlSanitizer
{
    public string Encode(string? value)
    {
        return HtmlEncoder.Default.Encode(value ?? string.Empty);
    }
}
