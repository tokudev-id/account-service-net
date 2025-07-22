using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace AccountService.Server.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = ConfigurationHelper.BuildConfiguration();
        var connectionString = configuration.GetConnectionString("ApplicationDBConnectionString")
            ?? throw new InvalidOperationException("Missing ApplicationDBConnectionString");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}

public class PersistedGrantDbContextFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
{
    public PersistedGrantDbContext CreateDbContext(string[] args)
    {
        var configuration = ConfigurationHelper.BuildConfiguration();
        var connectionString = configuration.GetConnectionString("ISGrantDBConnectionString")
            ?? throw new InvalidOperationException("Missing ISGrantDBConnectionString");

        var optionsBuilder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString),
            sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));

        var storeOptions = new OperationalStoreOptions();
        return new PersistedGrantDbContext(optionsBuilder.Options)
        {
            StoreOptions = storeOptions
        };
    }
}

public class ConfigurationDbContextFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
{
    public ConfigurationDbContext CreateDbContext(string[] args)
    {
        var configuration = ConfigurationHelper.BuildConfiguration();
        var connectionString = configuration.GetConnectionString("ISConfigDBConnectionString")
            ?? throw new InvalidOperationException("Missing ISConfigDBConnectionString");

        var optionsBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>();
        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString),
            sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));

        var storeOptions = new ConfigurationStoreOptions();
        return new ConfigurationDbContext(optionsBuilder.Options)
        {
            StoreOptions = storeOptions
        };
    }
}

internal static class ConfigurationHelper
{
    public static IConfiguration BuildConfiguration()
    {
        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            ?? Directory.GetCurrentDirectory();

        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{envName}.json", optional: true)
            .AddUserSecrets(typeof(Program).Assembly, optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
}