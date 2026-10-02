using SafeVaultApp.Services;

namespace SafeVaultApp.Middleware;

public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AuthService authService)
    {
        string? authorizationHeader = context.Request.Headers.Authorization.FirstOrDefault();
        string? token = null;

        if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
            authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = authorizationHeader["Bearer ".Length..].Trim();
        }

        var user = authService.ValidateToken(token);

        if (user is not null)
        {
            context.Items["CurrentUser"] = user;
        }

        await _next(context);
    }
}
