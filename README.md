# OrchardLite CMS — AWS Modernization Workshop

A legacy .NET Core 3.1 content management system used as the starting point for the AWS Enterprise Breakthrough Advance (EBA) Modernization Workshop. The workshop demonstrates transforming a legacy .NET application to .NET 8 using AWS Transform, with automated CI/CD deployment via CodePipeline.

## Workshop Flow

1. **Current State** — Deploy the legacy .NET Core 3.1 application on ECS Fargate with RDS MySQL
2. **Database Migration** — Use AWS DMS to migrate data from RDS MySQL to Aurora Serverless v2
3. **Application Transformation** — AWS Transform analyzes the codebase and upgrades to .NET 8
4. **Automated Deployment** — Transform checks the upgraded code into a new branch, triggering the CI/CD pipeline
5. **Validation** — The UI dynamically reflects the modernized state (framework version, database type, network security)

## Repository Structure

```
├── buildspec.yml                        # Unified build spec (works for both 3.1 and .NET 8)
├── OrchardLiteApp/
│   ├── OrchardLite.sln
│   ├── Dockerfile                       # Legacy Dockerfile (3.1)
│   └── OrchardLite.Web/
│       ├── Program.cs                   # Host builder entry point
│       ├── Startup.cs                   # MVC + DB initialization
│       ├── OrchardLite.Web.csproj       # Target framework (netcoreapp3.1 → net8.0)
│       ├── appsettings.json             # Connection string template
│       ├── Controllers/
│       │   └── HomeController.cs        # Content listing, health check, dynamic env detection
│       ├── Models/
│       │   └── ContentItem.cs           # Content data model
│       ├── Services/
│       │   └── DatabaseInitializer.cs   # Auto-creates DB, table, seeds 100 records
│       └── Views/
│           ├── Home/
│           │   ├── Index.cshtml         # Dashboard with dynamic modernization status
│           │   └── AllContent.cshtml    # Full content listing
│           └── Shared/
│               ├── _Layout.cshtml       # Dynamic layout (badges adapt to runtime)
│               └── Error.cshtml         # Error page
```

## How the Dual-Mode Build Works

The `buildspec.yml` is designed to work for both the legacy `main` branch and the transformed branch without any manual changes:

1. Reads `TargetFramework` from `.csproj` to detect `netcoreapp3.1` vs `net8.0`
2. Sets the appropriate SDK and runtime Docker image tags
3. Auto-fixes common AWS Transform compilation issues (e.g., `CS0136` duplicate variable declarations)
4. Generates a `Dockerfile.generated` matching the detected framework
5. Builds, pushes to ECR, and outputs `imagedefinitions.json` for ECS deployment

The Razor views are also self-adapting — `_Layout.cshtml` and `Index.cshtml` detect the .NET runtime version and database host at runtime to render the correct badges, warnings, and status indicators.

## Technology Stack

| Component | Legacy (main) | Transformed |
|-----------|---------------|-------------|
| Framework | .NET Core 3.1 | .NET 8 |
| Database  | RDS MySQL 8.0 | Aurora Serverless v2 |
| Network   | Public subnets | Private subnets with NAT |
| Connector | MySql.Data 8.0.33 | Updated by Transform |
| Config    | Startup.cs pattern | Modernized by Transform |

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `DB_HOST` | MySQL/Aurora hostname | `localhost` |
| `DB_PORT` | Database port | `3306` |
| `DB_NAME` | Database name | `OrchardLiteDB` |
| `DB_USER` | Database username | `root` |
| `DB_PASSWORD` | Database password | `password` |

## Endpoints

| Path | Description |
|------|-------------|
| `/` | Dashboard with content listing and modernization status |
| `/all-content` | All content items |
| `/health` | JSON health check (framework version, database type, phase) |

## Running Locally

```bash
cd OrchardLiteApp/OrchardLite.Web

export DB_HOST=localhost
export DB_PORT=3306
export DB_NAME=OrchardLiteDB
export DB_USER=root
export DB_PASSWORD=yourpassword

dotnet run
```

## Legacy Patterns (Intentional)

This application intentionally uses patterns that AWS Transform will detect and modernize:

- .NET Core 3.1 (EOL December 2022)
- Legacy `MySql.Data` connector
- Traditional `Startup.cs` configuration
- Manual ADO.NET data access (no ORM)
- Direct `new DatabaseInitializer()` instead of DI registration

## License

This project is licensed under the MIT-0 License. See the [LICENSE](LICENSE) file.
