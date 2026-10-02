using SafeVaultApp.Models;

namespace SafeVaultApp.Security;

public static class AuthorizationGuard
{
    public static bool HasAnyRole(AuthenticatedUser? user, params string[] allowedRoles)
    {
        if (user is null)
        {
            return false;
        }

        return allowedRoles.Any(role =>
            string.Equals(role, user.Role, StringComparison.OrdinalIgnoreCase));
    }

    public static IResult? RequireRole(HttpContext context, params string[] allowedRoles)
    {
        var user = context.Items["CurrentUser"] as AuthenticatedUser;

        if (user is null)
        {
            return Results.Unauthorized();
        }

        if (!HasAnyRole(user, allowedRoles))
        {
            return Results.Forbid();
        }

        return null;
    }

    public static AuthenticatedUser? GetCurrentUser(HttpContext context)
    {
        return context.Items["CurrentUser"] as AuthenticatedUser;
    }
}
