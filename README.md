# Colonial First State — Staff Directory REST API (.NET 8 Web API)

A production-quality REST API backend built with **ASP.NET Core 8**, **CQRS (MediatR)**, and **ADO.NET (SQLite / Turso)**, providing secure Staff Directory CRUD capabilities, audit metadata tracking, soft-deletion handling, JWT Bearer authentication, and cryptographic Refresh Token rotation.

Engineered for seamless integration with the companion **React + TypeScript Directory frontend**.

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Technology Stack](#2-technology-stack)
3. [Architecture](#3-architecture)
4. [Folder Structure](#4-folder-structure)
5. [Database Schema & SQL Table Scripts](#5-database-schema--sql-table-scripts)
6. [Audit Trail & Soft Delete Strategy](#6-audit-trail--soft-delete-strategy)
7. [CQRS Implementation](#7-cqrs-implementation)
8. [MediatR Request Pipeline](#8-mediatr-request-pipeline)
9. [Commands](#9-commands)
10. [Queries](#10-queries)
11. [Service Layer](#11-service-layer)
12. [Repository Layer](#12-repository-layer)
13. [ADO.NET Data Access](#13-adonet-data-access)
14. [JWT Authentication](#14-jwt-authentication)
15. [Refresh Token Lifecycle & Rotation](#15-refresh-token-lifecycle--rotation)
16. [Middleware Pipeline](#16-middleware-pipeline)
17. [Global Exception Handling](#17-global-exception-handling)
18. [Structured Logging & Correlation IDs](#18-structured-logging--correlation-ids)
19. [CORS Configuration](#19-cors-configuration)
20. [HTTPS & Transport Security](#20-https--transport-security)
21. [Swagger & OpenAPI with JWT](#21-swagger--openapi-with-jwt)
22. [Configuration Management](#22-configuration-management)
23. [Running Locally](#23-running-locally)
24. [React Frontend Integration](#24-react-frontend-integration)
25. [Security Considerations](#25-security-considerations)
26. [Future Improvements](#26-future-improvements)
27. [AI Tools Used](#27-ai-tools-used)

---

## 1. Project Overview

The **Staff Directory API** is designed following enterprise Clean Architecture standards:
* **Separation of Concerns**: Strict boundaries between Controllers, MediatR Handlers, Business Services, Repositories, and the SQLite / Turso database.
* **No ORM bloat**: Strictly avoids Entity Framework Core and Dapper in favor of high-performance, predictable ADO.NET (`Microsoft.Data.Sqlite`) with parameterized queries.
* **Audit Tracking & Soft Delete**: Every table includes `id`, `createdDate`, `createdBy`, `modifiedDate`, `modifiedBy`, `modifiedOn`, `deletedDate`, `deletedBy`, and `deletedOn`. Soft-deleted records are preserved in the database and automatically filtered out from all queries.
* **Enterprise Security**: JWT Bearer token authentication with SHA-256 hashed refresh tokens and PBKDF2 password hashing.
* **Dual Routing Compatibility**: `StaffController` exposes both `/api/staff` and `/api/users` routes for seamless plug-and-play frontend compatibility.

---

## 2. Technology Stack

* **Platform**: .NET 8 (LTS) / C# 12
* **Web Framework**: ASP.NET Core Web API
* **CQRS / Mediator**: MediatR 12.4.1
* **Database Engine**: SQLite / Turso Database
* **Data Access**: ADO.NET (`Microsoft.Data.Sqlite` 8.0.8)
* **Security & Auth**: `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.8, `System.IdentityModel.Tokens.Jwt` 8.0.2
* **Documentation**: Swashbuckle OpenAPI / Swagger 6.8.1
* **Logging**: Microsoft.Extensions.Logging `ILogger<T>`

---

## 3. Architecture

Every request travels through a strictly enforced, unidirectional pipeline:

```text
HTTP Request
     ↓
[ExceptionMiddleware]
     ↓
[RequestLoggingMiddleware] (Correlation ID & Timing)
     ↓
[HTTPS Redirection & CORS Policy]
     ↓
[JWT Authentication & Authorization]
     ↓
StaffController (Thin adapter, routes /api/staff & /api/users)
     ↓
IMediator (Send)
     ↓
Command / Query
     ↓
Command / Query Handler
     ↓
Business Service (Validation, Business Rules, Mapping, Audit)
     ↓
Repository Interface (IStaffRepository, IAuthRepository, etc.)
     ↓
ADO.NET (SqliteCommand, Parameters, DbConnectionFactory)
     ↓
SQLite / Turso Database (Soft-Delete & Audit Filtered)
```

---

## 4. Folder Structure

```text
Colonial First State - BackEnd/
├── Controllers/
│   ├── StaffController.cs                  # Secured Staff CRUD endpoints (/api/staff & /api/users)
│   └── AuthController.cs                   # Login and Refresh endpoints (/api/auth)
├── Contracts/
│   ├── Common/
│   │   └── ErrorResponse.cs                # Standardized API error contract
│   ├── Staff/
│   │   ├── Requests/
│   │   │   ├── CreateStaffRequest.cs
│   │   │   └── UpdateStaffRequest.cs
│   │   ├── Responses/
│   │   │   └── StaffResponse.cs            # DTO with audit & frontend fields
│   │   └── Models/
│   │       └── StaffModel.cs               # Domain model with audit fields
│   └── Auth/
│       ├── Requests/
│       │   ├── LoginRequest.cs
│       │   └── RefreshTokenRequest.cs
│       ├── Responses/
│       │   ├── LoginResponse.cs
│       │   └── RefreshTokenResponse.cs
│       └── Models/
│           ├── AuthUserModel.cs
│           └── RefreshTokenModel.cs
├── Application/
│   ├── Commands/
│   │   ├── Staff/
│   │   │   ├── CreateStaff/                # CreateStaffCommand & Handler
│   │   │   ├── UpdateStaff/                # UpdateStaffCommand & Handler
│   │   │   └── DeleteStaff/                # DeleteStaffCommand & Handler (Soft delete)
│   │   └── Auth/
│   │       ├── Login/                      # LoginCommand & Handler
│   │       └── RefreshToken/               # RefreshTokenCommand & Handler
│   └── Queries/
│       ├── Staff/
│       │   ├── GetStaff/                   # GetStaffQuery & Handler (Active records only)
│       │   └── GetStaffById/               # GetStaffByIdQuery & Handler (Active records only)
│       └── Auth/
├── Services/
│   ├── Interfaces/
│   │   ├── IStaffService.cs
│   │   ├── IAuthService.cs
│   │   └── ITokenService.cs
│   ├── StaffService.cs
│   ├── AuthService.cs
│   └── TokenService.cs
├── Repositories/
│   ├── Interfaces/
│   │   ├── IStaffRepository.cs
│   │   ├── IAuthRepository.cs
│   │   └── IRefreshTokenRepository.cs
│   ├── StaffRepository.cs
│   ├── AuthRepository.cs
│   └── RefreshTokenRepository.cs
├── Data/
│   ├── DatabaseConnection/
│   │   ├── IDbConnectionFactory.cs
│   │   └── DbConnectionFactory.cs          # Microsoft.Data.Sqlite factory
│   └── StoredProcedures/
│       └── StoredProcedureNames.cs
├── Infrastructure/
│   ├── Authentication/
│   │   ├── IJwtTokenService.cs
│   │   ├── JwtTokenService.cs
│   │   └── JwtOptions.cs
│   ├── Logging/
│   │   └── CorrelationLogScope.cs
│   └── Persistence/
│       └── SqlParameterHelper.cs
├── Middleware/
│   ├── ExceptionMiddleware.cs
│   └── RequestLoggingMiddleware.cs
├── Constants/
│   ├── ApiConstants.cs
│   ├── AuthConstants.cs
│   ├── DatabaseConstants.cs
│   └── ErrorConstants.cs
├── Extensions/
│   ├── ServiceCollectionExtensions.cs
│   ├── AuthenticationExtensions.cs
│   ├── SwaggerExtensions.cs
│   └── MiddlewareExtensions.cs
├── Validators/
│   ├── StaffValidator.cs
│   └── AuthValidator.cs
├── Exceptions/
│   ├── ApplicationException.cs
│   ├── NotFoundException.cs
│   ├── ValidationException.cs
│   └── UnauthorizedException.cs
├── sql/
│   ├── tables/
│   │   ├── Staff.sql                       # DDL with audit & soft-delete columns
│   │   ├── AuthUsers.sql                   # DDL with audit & credentials
│   │   └── RefreshTokens.sql               # DDL with token rotation & audit
│   └── seed.sql                            # Initial admin & sample data
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── UserDirectory.Api.csproj
├── UserDirectory.sln
└── README.md
```

---

## 5. Database Schema & SQL Table Scripts

All table DDL scripts are maintained under the `sql/tables/` directory for direct execution in Turso / SQLite:

* **[`sql/tables/Staff.sql`](file:///Users/ranjithsubramani/Documents/Ezhilarasan/Github/Colonial%20First%20State%20-%20BackEnd/sql/tables/Staff.sql)**:
  ```sql
  CREATE TABLE IF NOT EXISTS Staff (
      id TEXT PRIMARY KEY,
      name TEXT NOT NULL,
      age INTEGER NOT NULL,
      city TEXT NOT NULL,
      state TEXT NOT NULL,
      pincode TEXT NOT NULL,
      createdDate TEXT NOT NULL,
      createdBy TEXT NOT NULL,
      modifiedDate TEXT NULL,
      modifiedBy TEXT NULL,
      modifiedOn TEXT NULL,
      deletedDate TEXT NULL,
      deletedBy TEXT NULL,
      deletedOn TEXT NULL
  );
  ```

* **[`sql/tables/AuthUsers.sql`](file:///Users/ranjithsubramani/Documents/Ezhilarasan/Github/Colonial%20First%20State%20-%20BackEnd/sql/tables/AuthUsers.sql)**:
  Stores accounts with password hashes, salts, and audit trails.

* **[`sql/tables/RefreshTokens.sql`](file:///Users/ranjithsubramani/Documents/Ezhilarasan/Github/Colonial%20First%20State%20-%20BackEnd/sql/tables/RefreshTokens.sql)**:
  Stores hashed refresh tokens, expiration timestamps, revocation states, and audit trails.

* **[`sql/seed.sql`](file:///Users/ranjithsubramani/Documents/Ezhilarasan/Github/Colonial%20First%20State%20-%20BackEnd/sql/seed.sql)**:
  Inserts the default admin account (`admin` / `Password123!`) and sample staff directory records.

---

## 6. Audit Trail & Soft Delete Strategy

Every table implements complete lifecycle audit tracking:
* **Primary Key**: `id` (`TEXT`, GUID or string ID).
* **Creation**: `createdDate` (`TEXT` ISO-8601 UTC) and `createdBy` (`TEXT` user/identity).
* **Modification**: `modifiedDate` (`TEXT` ISO-8601 UTC), `modifiedBy` (`TEXT`), and `modifiedOn` (`TEXT`).
* **Soft Delete**: `deletedDate` (`TEXT` ISO-8601 UTC), `deletedBy` (`TEXT`), and `deletedOn` (`TEXT`).

### Soft Delete Rule
When an entity is deleted:
1. The record is **not** physically removed from the database (`DELETE FROM` is never executed).
2. The record is updated with:
   ```sql
   UPDATE Staff
   SET deletedDate = @deletedDate,
       deletedBy = @deletedBy,
       deletedOn = @deletedOn
   WHERE id = @id AND deletedDate IS NULL AND deletedBy IS NULL;
   ```
3. All query operations (`GetAll`, `GetById`, `Login`, token retrieval) include:
   ```sql
   WHERE deletedDate IS NULL AND deletedBy IS NULL
   ```
   Ensuring soft-deleted records are never returned to callers.

---

## 7. CQRS Implementation

The Command Query Responsibility Segregation (CQRS) pattern separates read operations from state mutations:
* **Commands**: Represent operations that modify state (`CreateStaff`, `UpdateStaff`, `DeleteStaff`, `Login`, `RefreshToken`). Commands capture the audit actor (`CreatedBy`, `ModifiedBy`, `DeletedBy`).
* **Queries**: Represent operations that read data without side effects (`GetStaff`, `GetStaffById`).
* **Isolation**: Commands and Queries are housed in dedicated parent folders (`Application/Commands/` and `Application/Queries/`).

---

## 8. MediatR Request Pipeline

Controllers interact with the domain exclusively through MediatR's `ISender`:
```csharp
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteStaff(string id, CancellationToken cancellationToken)
{
    var actor = User.Identity?.Name ?? "System";
    await _mediator.Send(new DeleteStaffCommand(id, actor), cancellationToken);
    return NoContent();
}
```

---

## 9. Commands

1. **`CreateStaffCommand`**: Accepts `CreateStaffRequest` and `CreatedBy`. Handled by `CreateStaffCommandHandler`, which invokes `IStaffService.CreateAsync`.
2. **`UpdateStaffCommand`**: Accepts `Id`, `UpdateStaffRequest`, and `ModifiedBy`. Handled by `UpdateStaffCommandHandler`, which invokes `IStaffService.UpdateAsync`.
3. **`DeleteStaffCommand`**: Accepts `Id` and `DeletedBy`. Handled by `DeleteStaffCommandHandler`, which performs a soft delete via `IStaffService.DeleteAsync`.
4. **`LoginCommand`**: Accepts `LoginRequest`. Handled by `LoginCommandHandler`, which invokes `IAuthService.LoginAsync`.
5. **`RefreshTokenCommand`**: Accepts `RefreshTokenRequest`. Handled by `RefreshTokenCommandHandler`, which invokes `IAuthService.RefreshTokenAsync`.

---

## 10. Queries

1. **`GetStaffQuery`**: Returns active staff as `IEnumerable<StaffResponse>`. Handled by `GetStaffQueryHandler`.
2. **`GetStaffByIdQuery`**: Accepts `Id` and returns `StaffResponse` for active staff. Handled by `GetStaffByIdQueryHandler`.

---

## 11. Service Layer

The Service Layer encapsulates all business rules, orchestration, input validation, and normalization:
* **`StaffService`**:
  * Enforces field validation (Name length, Age bounds 0–120, Pincode formatting).
  * Trims inputs.
  * Checks entity existence prior to update and delete, raising `NotFoundException` (HTTP 404).
  * Enforces soft-delete tracking (`DeletedDate`, `DeletedBy`).
* **`AuthService`**:
  * Validates credentials against stored PBKDF2 hash and salt.
  * Rejects invalid credentials with `UnauthorizedException` (HTTP 401).
* **`TokenService`**:
  * Orchestrates JWT creation and refresh token issuance.
  * Enforces token rotation upon refresh.

---

## 12. Repository Layer

Repositories handle database operations exclusively:
* **`StaffRepository`**: Executes parameterized queries on the `Staff` table with `WHERE deletedDate IS NULL AND deletedBy IS NULL`.
* **`AuthRepository`**: Executes `AuthUsers` lookup filtering out soft-deleted accounts.
* **`RefreshTokenRepository`**: Executes refresh token operations with soft-delete filtering.
* Repositories contain zero business logic and zero HTTP concepts.

---

## 13. ADO.NET Data Access

All data access is implemented directly with ADO.NET (`Microsoft.Data.Sqlite`):
* Connections are acquired from `IDbConnectionFactory`.
* `SqliteCommand` uses parameterized queries to eliminate SQL injection.
* Data readers (`SqliteDataReader`) read columns asynchronously with `CancellationToken`.
* Asynchronous resource disposal is guaranteed using `await using`.

---

## 14. JWT Authentication

JWT tokens are signed using HMAC-SHA256:
* **Claims**: `sub` (User ID), `unique_name` (Username), `role`, `email`, `jti`.
* **Validation**: Signature, Issuer, Audience, and Lifetime are validated with zero clock skew.
* **Header Format**: `Authorization: Bearer <access-token>`
* Protected endpoints:
  * `GET /api/staff` (and `/api/users`)
  * `GET /api/staff/{id}` (and `/api/users/{id}`)
  * `POST /api/staff` (and `/api/users`)
  * `PUT /api/staff/{id}` (and `/api/users/{id}`)
  * `DELETE /api/staff/{id}` (and `/api/users/{id}`)

---

## 15. Refresh Token Lifecycle & Rotation (Stateless JWT)

1. **Stateless JWT Tokens**: Refresh tokens are issued as cryptographically signed JWTs and are **not stored in the backend database**, eliminating database token lookups, schema bloat, and persistent session state.
2. **Single Secret Key**: A single unified secret key (`Jwt:Secret`) configured in `appsettings.json` is used for signing and validating both Access Tokens and Refresh Tokens.
3. **Cryptographic Claims**: The refresh JWT carries `sub` (User ID), `unique_name`, `token_type` (`refresh`), `token_family`, and a unique `jti`.
4. **Refresh Token Rotation**:
   * When `POST /api/auth/refresh-token` or `POST /api/auth/refresh` is requested with an existing refresh token JWT:
     1. The signature, issuer, audience, and lifetime are cryptographically verified using the single secret key.
     2. The token type claim is checked to ensure `token_type == 'refresh'`.
     3. The active user account status is validated against `AuthUsers`.
     4. A **brand-new Access Token** AND a **brand-new rotated Refresh Token JWT** (with refreshed lifetime and new `jti`) are generated and returned.
     5. The client seamlessly replaces the old refresh token with the rotated token.

---

## 16. Middleware Pipeline

```csharp
app.UseExceptionMiddleware();       // 1. Catches all downstream errors
app.UseRequestLoggingMiddleware();  // 2. Correlation ID & elapsed time
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(...);          // 3. API documentation
}
app.UseHttpsRedirection();          // 4. Force HTTPS
app.UseCors(...);                   // 5. Apply CORS headers
app.UseRouting();                   // 6. Match route
app.UseAuthentication();            // 7. Validate JWT
app.UseAuthorization();             // 8. Enforce permissions
app.MapControllers();               // 9. Execute action
```

---

## 17. Global Exception Handling

`ExceptionMiddleware` catches unhandled exceptions and outputs a uniform error contract:

```json
{
  "status": 400,
  "message": "One or more validation failures have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "errors": {
    "Name": ["Name must be between 2 and 100 characters in length."]
  },
  "correlationId": "b18b45f49eb048e49c76b91c530cb329",
  "timestamp": "2026-09-29T15:40:00Z"
}
```

* `ValidationException` -> `400 Bad Request`
* `UnauthorizedException` -> `401 Unauthorized`
* `NotFoundException` -> `404 Not Found`
* Generic `Exception` -> `500 Internal Server Error`

---

## 18. Structured Logging & Correlation IDs

* Every request is stamped with a Correlation ID (`X-Correlation-Id`).
* Duration (in ms) and status codes are logged upon request completion.
* Passwords, tokens, and secrets are strictly excluded from logs.

---

## 19. CORS Configuration

Configured for frontend development:
* Allowed Origins: `http://localhost:5173`, `http://localhost:3000`
* Allowed Methods: `GET`, `POST`, `PUT`, `DELETE`, `OPTIONS`
* Allowed Headers: `Authorization`, `Content-Type`
* Exposed Headers: `X-Correlation-Id`

---

## 20. HTTPS & Transport Security

* Strict HTTPS redirection enabled in pipeline (`app.UseHttpsRedirection()`).

---

## 21. Swagger & OpenAPI with JWT

Swagger is configured with JWT Bearer authentication:
1. Open the Swagger UI at `https://localhost:7001/`.
2. Authenticate via `POST /api/auth/login`.
3. Copy the `accessToken`.
4. Click **Authorize** and enter: `Bearer <your_access_token>`.

---

## 22. Configuration Management

Configured in `appsettings.json` and `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=userdirectory.db;Cache=Shared;"
  },
  "Jwt": {
    "Issuer": "UserDirectoryApi",
    "Audience": "UserDirectoryClient",
    "Secret": "A_Secure_Super_Secret_Key_For_JWT_Signing_Must_Be_Longer_Than_256_Bits_1234567890",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:3000"
    ]
  }
}
```

---

## 23. Running Locally

### 1. Execute SQL in Turso / SQLite
Execute the scripts in `sql/tables/` and optionally `sql/seed.sql`:
* `sql/tables/Staff.sql`
* `sql/tables/AuthUsers.sql`
* `sql/tables/RefreshTokens.sql`
* `sql/seed.sql`

### 2. Build & Run API
```bash
dotnet restore
dotnet build
dotnet run
```
The API will launch and serve Swagger UI at `https://localhost:7001` or `http://localhost:5000`.

---

## 24. React Frontend Integration

Compatible out-of-the-box with `Colonial First State - FrontEnd`:
* Endpoints supported: both `/api/staff` and `/api/users` routes are active on `StaffController`.
* Fields returned: `id`, `name`, `age`, `city`, `state`, `pincode`, `createdDate`, `modifiedDate`, `createdAt`, `updatedAt`.
1. In `Colonial First State - FrontEnd/.env`:
   ```env
   VITE_API_BASE_URL=https://localhost:7001/api
   VITE_USE_MOCK_API=false
   ```
2. The frontend Axios interceptor automatically manages tokens, proactive refresh, and 401 retries.

---

## 25. Security Considerations

* **Parameterized Queries**: 100% immune to SQL injection through typed `SqliteParameter` bindings.
* **Hashed Refresh Tokens**: Only SHA-256 hashes are persisted.
* **PBKDF2 Password Storage**: HMAC-SHA256 with 100,000 iterations and cryptographic salt.
* **Soft Delete Safety**: Records are never erased; audit tracks who deleted and when.

---

## 26. Future Improvements

* Add Turso libSQL native HTTP client or replication support.
* Add Rate Limiting middleware.
* Add Health Checks endpoint (`/healthz`).

---

## 27. AI Tools Used

Developed with **Antigravity AI (Google DeepMind)**.
