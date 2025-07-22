using AccountService.Server.Data;
using AccountService.Server.Identity;
using AccountService.Server.Models;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace AccountService.Server.Extensions
{
    public static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;
            var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

            // DB and Identity Configuration
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(configuration.GetConnectionString("ApplicationDBConnectionString"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("ApplicationDBConnectionString"))));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = false;
                // Add other identity options...
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserManager<UserManager<ApplicationUser>>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddDefaultTokenProviders();

            // IdentityServer Configuration
            services.AddIdentityServer(options =>
            {
                options.IssuerUri = configuration["IdentityServer:IssuerUri"];
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseMySql(configuration.GetConnectionString("ISConfigDBConnectionString"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("ISConfigDBConnectionString")),
                    sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseMySql(configuration.GetConnectionString("ISGrantDBConnectionString"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("ISGrantDBConnectionString")),
                    sql => sql.MigrationsAssembly(migrationsAssembly));
                options.EnableTokenCleanup = true;
            })
            .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<AccountProfileService>();

            // Register all your other services
            services.RegisterApplicationServices(configuration);

            services.AddControllersWithViews();
            services.AddSwaggerGen();

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            if (!app.Environment.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Use a descriptive policy name
            app.UseCors("DefaultCorsPolicy");

            app.UseIdentityServer(); // UseAuthentication is called inside UseIdentityServer
            app.UseAuthorization();

            app.MapDefaultControllerRoute();
            app.MapFallbackToFile("/index.html");

            return app;
        }

        /// <summary>
        /// A central place for all your custom application services.
        /// </summary>
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // CORS Policy
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", policy =>
                {
                    // It's better to be specific than allowing any origin in production
                    var allowedDomain = configuration.GetSection("AllowedOrigins:Domain").Get<string[]>();
                    policy.WithOrigins(allowedDomain)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Redis and Data Protection
            var redisConnectionString = configuration.GetConnectionString("Redis");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "AccountService:Sessions:";
            });

            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddDataProtection().PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

            // Singleton Services
            //services.AddSingleton<ICloudStorage, GoogleCloudStorage>();
            // Add other singletons...

            // Scoped Services
            //services.AddScoped<IAccountService, AccountService>();
            //services.AddScoped<IEmailUtil, EmailUtil>();
            // Add other scoped services...

            return services;
        }

        /// <summary>
        /// Applies any pending migrations for the context to the database.
        /// </summary>
        /// <summary>
        /// Applies any pending migrations for the context to the database.
        /// Will create the database if it does not already exist.
        /// </summary>
        /// <param name="app">The WebApplication instance.</param>
        public static void InitializeDatabase(this WebApplication app)
        {
            // Create a new scope to retrieve services
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            // Get a logger to provide context
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Initializing and migrating databases...");

            // Migrate each DbContext
            MigrateDbContext<ApplicationDbContext>(services, logger);
            MigrateDbContext<PersistedGrantDbContext>(services, logger);
            MigrateDbContext<ConfigurationDbContext>(services, logger);

            logger.LogInformation("Database initialization complete.");
        }

        /// <summary>
        /// A generic helper method to apply migrations for any DbContext.
        /// </summary>
        /// <typeparam name="TContext">The type of the DbContext.</typeparam>
        /// <param name="services">The service provider to resolve services from.</param>
        /// <param name="logger">The logger for logging migration status.</param>
        private static void MigrateDbContext<TContext>(IServiceProvider services, ILogger logger) where TContext : DbContext
        {
            var contextName = typeof(TContext).Name;

            try
            {
                logger.LogInformation("Checking pending migrations for {DbContextName}", contextName);
                var context = services.GetRequiredService<TContext>();

                if (context.Database.GetPendingMigrations().Any())
                {
                    logger.LogInformation("Applying migrations for {DbContextName}...", contextName);
                    context.Database.Migrate();
                    logger.LogInformation("{DbContextName} has been successfully migrated.", contextName);
                }
                else
                {
                    logger.LogInformation("{DbContextName} is up to date. No migrations to apply.", contextName);
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "An error occurred while migrating the {DbContextName} database.", contextName);
                // Optionally, rethrow or handle the exception as needed for your startup flow
                throw;
            }
        }
    }
}
