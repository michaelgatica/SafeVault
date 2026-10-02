using SafeVaultApp.Data;
using SafeVaultApp.Middleware;
using SafeVaultApp.Models;
using SafeVaultApp.Security;
using SafeVaultApp.Services;
using SafeVaultApp.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<InputValidator>();
builder.Services.AddSingleton<HtmlSanitizer>();
builder.Services.AddSingleton<SafeVaultRepository>();

var app = builder.Build();

await app.Services.GetRequiredService<SafeVaultRepository>().InitializeAsync();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<TokenAuthenticationMiddleware>();

app.MapGet("/", () => Results.Ok(new
{
    application = "SafeVault",
    purpose = "Secure coding demo for validation, SQL injection prevention, authentication, RBAC, XSS protection, and tests."
}));

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow
}));

app.MapPost("/api/auth/login", (LoginRequest request, AuthService authService) =>
{
    var user = authService.Login(request.Username, request.Password);

    return user is null
        ? Results.Unauthorized()
        : Results.Ok(new
        {
            user.Username,
            user.Role,
            user.Token
        });
});

app.MapGet("/api/vault/records", async (
    HttpContext context,
    SafeVaultRepository repository,
    HtmlSanitizer sanitizer,
    string? search) =>
{
    var authorizationResult = AuthorizationGuard.RequireRole(context, "Admin", "User");

    if (authorizationResult is not null)
    {
        return authorizationResult;
    }

    var records = await repository.SearchRecordsAsync(search);

    var safeRecords = records.Select(record => new
    {
        record.Id,
        Title = sanitizer.Encode(record.Title),
        SecretValue = sanitizer.Encode(record.SecretValue),
        Notes = sanitizer.Encode(record.Notes),
        CreatedBy = sanitizer.Encode(record.CreatedBy),
        record.CreatedAt
    });

    return Results.Ok(safeRecords);
});

app.MapPost("/api/vault/records", async (
    HttpContext context,
    VaultRecordRequest request,
    SafeVaultRepository repository,
    InputValidator validator,
    HtmlSanitizer sanitizer) =>
{
    var authorizationResult = AuthorizationGuard.RequireRole(context, "Admin", "User");

    if (authorizationResult is not null)
    {
        return authorizationResult;
    }

    var errors = validator.ValidateVaultRecord(request);

    if (errors.Any())
    {
        return Results.ValidationProblem(errors);
    }

    var currentUser = AuthorizationGuard.GetCurrentUser(context)!;

    var record = new VaultRecord
    {
        Title = sanitizer.Encode(request.Title),
        SecretValue = sanitizer.Encode(request.SecretValue),
        Notes = sanitizer.Encode(request.Notes),
        CreatedBy = currentUser.Username,
        CreatedAt = DateTime.UtcNow
    };

    var savedRecord = await repository.AddRecordAsync(record);

    return Results.Created($"/api/vault/records/{savedRecord.Id}", savedRecord);
});

app.MapGet("/api/admin/dashboard", (HttpContext context) =>
{
    var authorizationResult = AuthorizationGuard.RequireRole(context, "Admin");

    if (authorizationResult is not null)
    {
        return authorizationResult;
    }

    return Results.Ok(new
    {
        message = "Welcome to the SafeVault admin dashboard.",
        accessLevel = "Admin"
    });
});

app.Run();
