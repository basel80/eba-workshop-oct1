# OrchardLite.Web — Application Details

ASP.NET Core MVC application for the OrchardLite CMS. See the [root README](../README.md) for workshop context and build instructions.

## Database Schema

```sql
CREATE TABLE ContentItems (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Summary TEXT,
    Body LONGTEXT,
    ContentType VARCHAR(50) NOT NULL DEFAULT 'BlogPost',
    AuthorId INT,
    PublishedDate DATETIME NOT NULL,
    ViewCount INT DEFAULT 0,
    IsPublished BOOLEAN DEFAULT TRUE,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_published_date (PublishedDate),
    INDEX idx_content_type (ContentType)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

The `DatabaseInitializer` service auto-creates this table and seeds 100 sample records on first startup.

## Docker Build

```bash
# Legacy (from this directory)
docker build -t orchardlite-cms:latest .

# Run
docker run -p 80:80 \
  -e DB_HOST=your-db-host \
  -e DB_PORT=3306 \
  -e DB_NAME=OrchardLiteDB \
  -e DB_USER=your-user \
  -e DB_PASSWORD=your-password \
  orchardlite-cms:latest
```

Note: The CI/CD pipeline generates its own `Dockerfile.generated` that adapts to the detected .NET version. The checked-in `Dockerfile` targets .NET Core 3.1 for the legacy branch.
