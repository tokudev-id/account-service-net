using AccountService.Server.Extensions;
using Serilog;

// Configure Serilog for structured logging first
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add custom configuration
    builder.AddCustomConfiguration(args);

    // Add Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration));

    // Configure all your services
    var app = builder.ConfigureServices();

    // Configure the HTTP request pipeline
    app.ConfigurePipeline();

    // Initialize the database
    app.InitializeDatabase();

    // This starts the web server and keeps the application alive
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}