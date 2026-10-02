# SafeVault

SafeVault is a secure ASP.NET Core practice project that combines three secure-coding activities into one final project. The application demonstrates input validation, SQL injection prevention, authentication, role-based access control, XSS protection, middleware, and security tests.

## Project Structure

```text
SafeVault/
├── SafeVault.sln
├── README.md
├── SECURITY_SUMMARY.md
├── COPILOT_SUMMARY.md
├── TEST_PLAN.md
├── src/SafeVaultApp/
└── tests/SafeVaultTests/
```

## Features

- Secure input validation for user-submitted data.
- Parameterized SQL queries using `Microsoft.Data.Sqlite`.
- Authentication with secure PBKDF2 password hashing.
- Token-based authorization with role-based access control.
- Admin-only route for sensitive actions.
- XSS protection through HTML encoding.
- Security headers middleware.
- Request logging middleware.
- Centralized error handling middleware.
- Unit tests for validation, SQL injection prevention, password hashing, XSS mitigation, and authorization logic.

## Run the Application

```bash
cd src/SafeVaultApp
dotnet restore
dotnet run
```

The launch profile uses:

```text
http://localhost:5078
```

## Demo Accounts

| Username | Password | Role |
|---|---|---|
| `admin` | `Admin!234` | Admin |
| `user` | `User!234` | User |

## Authentication

Log in by sending a POST request to:

```text
POST /api/auth/login
```

Example body:

```json
{
  "username": "admin",
  "password": "Admin!234"
}
```

The response includes a token. Use that token with protected endpoints:

```text
Authorization: Bearer <token>
```

## Run Tests

```bash
dotnet test
```

## Important Endpoints

| Method | Endpoint | Purpose |
|---|---|---|
| `POST` | `/api/auth/login` | Authenticates a user |
| `GET` | `/api/vault/records` | Retrieves records for authorized users |
| `POST` | `/api/vault/records` | Creates a secure record |
| `GET` | `/api/admin/dashboard` | Admin-only route |
| `GET` | `/health` | Health check |
