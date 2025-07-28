using AccountService.Server.Data;
using AccountService.Server.Identity;
using AccountService.Server.Middleware;
using AccountService.Server.Models;
using AccountService.Server.Repositories.User;
using AccountService.Server.Services.Authentication;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Net;

namespace AccountService.Server.Extensions
{
    public static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;
            var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

            var redisServerSection = configuration.GetSection("RedisServer");
            var redisConfig = redisServerSection.GetSection("RedisCacheConfiguration").Get<string>();

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
            .AddProfileService<AccountProfileService>()
            .AddResourceOwnerValidator<PinResourceOwnerPasswordValidator>();

            SetupSession(services, redisConfig);
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
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.MapFallbackToFile("/index.html");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseMiddleware<SecurityHeadersMiddleware>();

            // Use a descriptive policy name
            app.UseCors("DefaultCorsPolicy");
            app.UseSession();
            //app.Use(async (ctx, next) =>
            //{
            //    var duendeOptions = configuration.GetSection("IdentityServer").Get<IdentityServerOptions>();
            //    ctx.RequestServices.GetRequiredService<IServerUrls>().Origin = duendeOptions.IssuerUri;
            //    await next();
            //});

            app.UseAuthentication();

            app.UseIdentityServer(); // UseAuthentication is called inside UseIdentityServer
            app.UseAuthorization();

            app.MapDefaultControllerRoute();

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

            // Singleton Services
            //services.AddSingleton<ICloudStorage, GoogleCloudStorage>();
            // Add other singletons...

            services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
            // scoped Repository
            services.AddScoped<IUserRepository, UserRepository>();
            // Scoped Services
            services.AddScoped<Services.Account.IAccountService, Services.Account.AccountService>();
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
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            Console.WriteLine("Initializing and migrating databases...");

            MigrateDbContext<ApplicationDbContext>(services);
            MigrateDbContext<PersistedGrantDbContext>(services);
            MigrateDbContext<ConfigurationDbContext>(services);

            Console.WriteLine("Database initialization complete.");
        }


        /// <summary>
        /// A generic helper method to apply migrations for any DbContext.
        /// </summary>
        /// <typeparam name="TContext">The type of the DbContext.</typeparam>
        /// <param name="services">The service provider to resolve services from.</param>
        /// <param name="logger">The logger for logging migration status.</param>
        private static void MigrateDbContext<TContext>(IServiceProvider services) where TContext : DbContext
        {
            var contextName = typeof(TContext).Name;

            try
            {
                Console.WriteLine($"Checking pending migrations for {contextName}");
                var context = services.GetRequiredService<TContext>();

                if (context.Database.GetPendingMigrations().Any())
                {
                    Console.WriteLine($"Applying migrations for {contextName}...");
                    context.Database.Migrate();
                    Console.WriteLine($"{contextName} has been successfully migrated.");
                }
                else
                {
                    Console.WriteLine($"{contextName} is up to date. No migrations to apply.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CRITICAL: An error occurred while migrating the {contextName} database.");
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        private static void SetupSession(IServiceCollection services, string redisConfig)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.Secure = CookieSecurePolicy.Always;
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LogoutPath = "/api/auth/logout";
                options.LoginPath = "/login";
                options.Events.OnRedirectToLogin = context =>
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                    return Task.CompletedTask;
                };
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SessionStore = new RedisCacheTicketStore(new RedisCacheOptions()
                {
                    Configuration = redisConfig
                });
            });

            services.AddSession(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
        }

    }
}
