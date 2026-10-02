# SafeVault Test Plan

## 1. GitHub Repository

Confirm the project is uploaded to a public GitHub repository and includes:

- `SafeVault.sln`
- `README.md`
- `SECURITY_SUMMARY.md`
- `COPILOT_SUMMARY.md`
- `TEST_PLAN.md`
- `src/SafeVaultApp`
- `tests/SafeVaultTests`

## 2. Input Validation Tests

Run:

```bash
dotnet test
```

Expected results:

- Empty titles are rejected.
- Long titles are rejected.
- SQL injection-like strings are rejected.
- Required fields must be present.

## 3. SQL Injection Prevention Tests

The tests use a temporary SQLite database and confirm that malicious search input such as:

```text
' OR 1=1 --
```

does not return all records.

## 4. Authentication Tests

The tests verify that a valid password matches its hash and that an invalid password fails verification.

## 5. Authorization Tests

The tests verify:

- Admin users satisfy admin role requirements.
- Standard users do not satisfy admin role requirements.
- Standard users can still satisfy user-level access requirements.

## 6. XSS Protection Tests

The tests verify that script tags are encoded instead of returned as executable HTML.

## 7. Manual API Test

Run the app:

```bash
cd src/SafeVaultApp
dotnet run
```

Then test login:

```http
POST http://localhost:5078/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin!234"
}
```

Use the returned token to call protected routes:

```http
GET http://localhost:5078/api/vault/records
Authorization: Bearer <token>
```
