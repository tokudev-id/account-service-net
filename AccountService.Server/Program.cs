using AccountService.Server.Extensions;
using Serilog;

try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .CreateBootstrapLogger();

    Log.Information("==== Application Starting ====");

    var builder = WebApplication.CreateBuilder(args);
    Log.Information("Builder created");

    Log.Information("Adding custom configuration...");
    builder.AddCustomConfiguration(args);
    Log.Information("Custom configuration added");

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    Log.Information("Configuring services...");
    var app = builder.ConfigureServices();
    Log.Information("Services configured");

    Log.Information("Configuring pipeline...");
    app.ConfigurePipeline();
    Log.Information("Pipeline configured");

    Log.Information("Initializing database...");
    try
    {
        app.InitializeDatabase();
        Log.Information("Database initialized");
    }
    catch (Exception dbEx)
    {
        Log.Error(dbEx, "Database initialization failed");
        throw; // rethrow so app doesn't continue silently
    }

    Log.Information("Running Kestrel...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    Console.WriteLine($"[FATAL] {ex}"); // fallback if Serilog fails
}
finally
{
    Log.Information("==== Application Shutting Down ====");
    Log.CloseAndFlush();
}
