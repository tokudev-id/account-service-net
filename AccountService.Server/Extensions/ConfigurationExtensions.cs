namespace AccountService.Server.Extensions
{
    public static class ConfigurationExtensions
    {
        public static WebApplicationBuilder AddCustomConfiguration(this WebApplicationBuilder builder, string[] args)
        {
            // --- This section replicates getting the environment from a custom source ---
            // In modern .NET, ASPNETCORE_ENVIRONMENT is the standard.
            // We'll honor it, but allow an override from args like your old app might have.
            var environmentName = builder.Environment.EnvironmentName;
            Console.WriteLine($"Environment detected from ASPNETCORE_ENVIRONMENT: {environmentName}");

            // --- Clear default providers to take full control, just like the old code ---
            builder.Configuration.Sources.Clear();

            // --- Define the list of base appsettings files you want to load ---
            var appSettingsFiles = new List<string> { "appsettings" }; // Add other base files like "customsettings" if needed

            // --- Load configurations in the desired order ---
            // 1. Base files
            foreach (var filepath in appSettingsFiles)
            {
                builder.Configuration.AddJsonFile($"{filepath}.json", optional: false, reloadOnChange: true);
            }

            // 2. Environment-specific files (e.g., appsettings.Development.json)
            foreach (var filepath in appSettingsFiles)
            {
                builder.Configuration.AddJsonFile($"{filepath}.{environmentName}.json", optional: true, reloadOnChange: true);
            }

            // 3. User Secrets (in Development environment)
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>(optional: true);
            }

            // 4. Environment Variables (e.g., from Docker, Kubernetes)
            builder.Configuration.AddEnvironmentVariables();

            // 5. Command-line arguments
            builder.Configuration.AddCommandLine(args);

            // --- Log the final configuration providers for visibility ---
            Console.WriteLine("\n--- Active Configuration Providers ---");
            foreach (var source in builder.Configuration.Sources)
            {
                Console.WriteLine(source.ToString());
            }
            Console.WriteLine("------------------------------------\n");

            return builder;
        }
    }
}
