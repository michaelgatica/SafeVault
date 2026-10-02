# Security Summary

This project addresses the SafeVault assignment requirements by combining secure coding, authentication, authorization, vulnerability remediation, and tests.

## Vulnerabilities Identified

### SQL Injection

The original scenario included unsafe string concatenation in SQL queries. This could allow attackers to inject malicious SQL through form fields or search inputs.

### Cross-Site Scripting

The original scenario also included the risk of displaying user-submitted content without encoding. If an attacker submitted a script tag, the application could display and execute unsafe content in the browser.

### Weak Access Control

Sensitive routes needed authentication and role-based access control to ensure that only authorized users could view or modify protected information.

## Fixes Applied

### Input Validation

The application validates all user-submitted record data. Titles, secret values, and notes are checked for length, required values, and suspicious attack patterns.

### Parameterized Queries

The repository uses parameterized SQL commands with `SqliteParameter` values instead of string concatenation. This prevents user input from being interpreted as executable SQL.

### Authentication

The application includes login functionality through `AuthService`. Passwords are stored using PBKDF2 hashing with salts rather than plain text.

### Authorization and RBAC

The application uses role-based access checks. Standard users can access user-level routes, while admin-only routes require the `Admin` role.

### XSS Protection

User-submitted output is encoded using `HtmlEncoder` before being returned to the client. This prevents script tags and unsafe HTML from being executed.

### Middleware

The middleware pipeline includes centralized error handling, security headers, request logging, and token authentication.

## Tests

The test project verifies:

- SQL injection-like input is rejected or safely handled.
- Parameterized queries do not return unintended records.
- Password hashing and verification work correctly.
- XSS content is encoded.
- Role-based checks distinguish between admin and standard users.
