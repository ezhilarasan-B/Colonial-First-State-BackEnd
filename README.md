# Colonial First State — Staff & Client Directory API (.NET 8 Web API)

A production-quality REST API backend built with **ASP.NET Core 8**, **CQRS (MediatR)**, **FluentValidation**, and **ADO.NET (SQLite / Turso)**. It provides comprehensive Staff Directory and Client Management, audit metadata tracking, soft-deletion handling, JWT Bearer authentication, and role-based access control.

Designed for seamless integration with the companion **React + TypeScript Directory frontend**.

---

## 1. Technology Stack

* **Platform**: .NET 8 (LTS) / C# 12
* **Web Framework**: ASP.NET Core Web API
* **CQRS / Mediator**: MediatR (with validation pipeline behavior)
* **Validation**: FluentValidation (automatic request validation)
* **Database**: SQLite / Turso
* **Data Access**: ADO.NET (`Microsoft.Data.Sqlite`) with parameterized queries
* **Authentication & Authorization**: JWT Bearer token authentication with role-based policies (`Admin`, `StaffReadOnly`)
* **API Documentation**: Swagger / OpenAPI with JWT Bearer support
* **Logging**: Structured logging with Correlation ID tracking

---

## 2. Architecture & Design Principles

* **Clean Architecture**: Source code organized under `src/` with clear separation among Controllers, Application layer (Commands/Queries), Services, Repositories, and Contracts.
* **CQRS Pattern**: Segregated read queries and state-mutating commands via MediatR handlers.
* **Validation Pipeline**: FluentValidation rules execute automatically via MediatR `ValidationBehavior` before reaching business handlers.
* **Direct ADO.NET**: Eliminates ORM overhead; uses parameterized SQLite commands to guarantee performance and security against SQL injection.
* **Soft Deletion & Audit Trail**: Records track creation, modification, and deletion metadata (`createdDate`, `createdBy`, `modifiedDate`, `modifiedBy`, `deletedDate`, `deletedBy`). Deleted records are excluded from standard queries.
* **API Versioning**: All endpoints follow the `/api/v1/` route prefix, managed through centralized constants.

---

## 3. Project Structure

```text
Colonial-First-State-BackEnd/
├── src/
│   ├── Application/
│   │   ├── Behaviors/               # Validation pipeline behavior
│   │   ├── Commands/
│   │   │   ├── Auth/                # Login & Refresh token commands
│   │   │   ├── AuthUser/            # Auth user management commands
│   │   │   └── Staff/               # Staff CRUD & bulk-delete commands
│   │   └── Queries/
│   │       ├── AuthUser/            # Auth user queries
│   │       └── Staff/               # Staff retrieval queries
│   ├── Constants/                   # API, Auth, Database, and Error constants
│   ├── Contracts/                   # Requests, Responses, and Domain Models
│   │   ├── Auth/
│   │   ├── AuthUser/
│   │   ├── Client/
│   │   ├── Common/                  # ErrorResponse, PagedResult, BulkDeleteRequest
│   │   ├── Interfaces/              # Repository & Service contracts
│   │   └── Staff/
│   ├── Controllers/                 # REST API Controllers (v1)
│   │   ├── AuthController.cs        # Login & token refresh
│   │   ├── AuthUserController.cs    # Auth user administration (Admin only)
│   │   ├── ClientController.cs      # Client management & bulk delete
│   │   └── StaffController.cs       # Staff directory & bulk delete
│   ├── Data/
│   │   └── DatabaseConnection/      # IDbConnectionFactory & SQLite connection factory
│   ├── Exceptions/                  # Domain exceptions (NotFound, Validation, etc.)
│   ├── Extensions/                  # Service & middleware registration, Swagger, Auth policies
│   ├── Infrastructure/
│   │   ├── Authentication/          # JWT token service & options
│   │   └── Logging/                 # Correlation ID logging scope
│   ├── Middleware/                  # Exception handling & request logging middleware
│   ├── Repositories/                # ADO.NET data access implementations
│   ├── Services/                    # Business service implementations
│   └── Validators/                  # FluentValidation validators
├── Properties/
├── appsettings.json
├── appsettings.Development.json
├── UserDirectory.Api.csproj
└── README.md
```

---

## 4. Key Modules & Business Rules

### Authentication & Authorization
* **JWT Bearer Authentication**: Access tokens and rotated refresh tokens.
* **Role-Based Permissions**:
  * `Admin`: Full read and write permissions across Staff, Clients, and Auth Users.
  * `StaffReadOnly`: Read-only access for Staff and Clients; write and delete operations are restricted.
* **Separated Auth Controllers**:
  * `AuthController`: Public endpoints for user login and token refresh.
  * `AuthUserController`: Restricted management endpoints for administrative user accounts.

### Staff Directory
* Paginated and unpaginated staff listings.
* Add, update, and soft-delete operations.
* **Staff-to-Client Deletion Guard**: A staff member assigned to any active client cannot be deleted. The system validates assignments and returns an informative error if deletion is attempted.
* **Bulk Delete**: Supports deleting multiple staff members in a single request while validating client assignment constraints.

### Client Management
* Full client record lifecycle (create, read, update, soft-delete).
* **Phone Number Validation**: Enforces exactly 10 digits across frontend, backend validation, and database constraints.
* **Bulk Delete**: Multi-record deletion support.

### Error Handling & Validation
* Uniform error response contract containing HTTP status, message, error code, correlation ID, and timestamp.
* Automatic translation of FluentValidation errors into structured validation responses.

---

## 5. API Overview

All routes are versioned under `/api/v1/`:

| Module | Route | Access |
|---|---|---|
| **Auth** | `/api/v1/auth/login` | Anonymous |
| **Auth** | `/api/v1/auth/refresh-token` | Anonymous |
| **Auth Users** | `/api/v1/authusers` | Admin |
| **Staff** | `/api/v1/staff` | Authenticated (StaffRead / StaffWrite) |
| **Staff** | `/api/v1/staff/bulk-delete` | Authenticated (StaffWrite) |
| **Client** | `/api/v1/client` | Authenticated (ClientRead / ClientWrite) |
| **Client** | `/api/v1/client/bulk-delete` | Authenticated (ClientWrite) |

Interactive documentation is available via Swagger UI when running in development.

---

## 6. Configuration

Application settings are configured in `appsettings.json` or environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=userdirectory.db;Cache=Shared;"
  },
  "Jwt": {
    "Issuer": "UserDirectoryApi",
    "Audience": "UserDirectoryClient",
    "Secret": "<YOUR_JWT_SIGNING_SECRET_MIN_256_BITS>",
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

> **Note**: Always replace placeholder secrets with secure keys in production environments.

---

## 7. Running Locally

### Prerequisites
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Steps
1. Restore dependencies and build the solution:
   ```bash
   dotnet restore
   dotnet build
   ```

2. Run the API:
   ```bash
   dotnet run --launch-profile "http"
   ```

3. Access the service:
   * **API Base URL**: `http://localhost:5200`
   * **Swagger Documentation**: `http://localhost:5200/swagger/index.html` (auto-opens when configured)
