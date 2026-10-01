using System;
using Microsoft.Extensions.Configuration;

namespace OrchardLite.Web.Services
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString(bool includeDatabase = true);
    }

    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString(bool includeDatabase = true)
        {
            var connStr = _configuration.GetConnectionString("OrchardLiteDB");

            var dbHost = Environment.GetEnvironmentVariable("DB_HOST")
                ?? throw new InvalidOperationException("Required environment variable 'DB_HOST' is not set.");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT")
                ?? throw new InvalidOperationException("Required environment variable 'DB_PORT' is not set.");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME")
                ?? throw new InvalidOperationException("Required environment variable 'DB_NAME' is not set.");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER")
                ?? throw new InvalidOperationException("Required environment variable 'DB_USER' is not set.");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")
                ?? throw new InvalidOperationException("Required environment variable 'DB_PASSWORD' is not set.");

            connStr = connStr
                .Replace("${DB_HOST}", dbHost)
                .Replace("${DB_PORT}", dbPort)
                .Replace("${DB_NAME}", includeDatabase ? dbName : "")
                .Replace("${DB_USER}", dbUser)
                .Replace("${DB_PASSWORD}", dbPassword);

            if (!includeDatabase)
            {
                connStr = connStr.Replace("Database=;", "");
            }

            return connStr;
        }
    }
}
