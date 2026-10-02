using SafeVaultApp.Models;

namespace SafeVaultApp.Validation;

public class InputValidator
{
    private static readonly string[] SuspiciousPatterns =
    {
        "--",
        ";--",
        "/*",
        "*/",
        " or ",
        " and ",
        "<script",
        "</script",
        "javascript:",
        "onerror=",
        "onload="
    };

    public Dictionary<string, string[]> ValidateVaultRecord(VaultRecordRequest? request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request is null)
        {
            errors["Request"] = new[] { "Request body is required." };
            return errors;
        }

        ValidateRequiredText(errors, "Title", request.Title, 100);
        ValidateRequiredText(errors, "SecretValue", request.SecretValue, 250);
        ValidateOptionalText(errors, "Notes", request.Notes, 500);

        return errors;
    }

    private static void ValidateRequiredText(
        Dictionary<string, string[]> errors,
        string field,
        string? value,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors[field] = new[] { $"{field} is required." };
            return;
        }

        ValidateCommonText(errors, field, value, maxLength);
    }

    private static void ValidateOptionalText(
        Dictionary<string, string[]> errors,
        string field,
        string? value,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        ValidateCommonText(errors, field, value, maxLength);
    }

    private static void ValidateCommonText(
        Dictionary<string, string[]> errors,
        string field,
        string value,
        int maxLength)
    {
        string trimmedValue = value.Trim();

        if (trimmedValue.Length > maxLength)
        {
            errors[field] = new[] { $"{field} cannot exceed {maxLength} characters." };
            return;
        }

        string normalized = $" {trimmedValue.ToLowerInvariant()} ";

        if (SuspiciousPatterns.Any(pattern => normalized.Contains(pattern)))
        {
            errors[field] = new[] { $"{field} contains potentially unsafe content." };
        }
    }
}
