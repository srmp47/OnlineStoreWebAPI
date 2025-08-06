using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OnlineStoreWebAPI.DBContext;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using OnlineStoreWebAPI.Mapping;

namespace TestProject.Integration
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private static bool _autoMapperInitialized = false;
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"Authentication:SecretForKey", TestAuthHelper.TestSecretKey},
                {"Authentication:Issuer", TestAuthHelper.TestIssuer},
                {"Authentication:Audience", TestAuthHelper.TestAudience}
            });
            });
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<OnlineStoreDBContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database
                services.AddDbContext<OnlineStoreDBContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Prevent AutoMapper from being initialized multiple times
                if (!_autoMapperInitialized)
                {
                    // Remove any existing AutoMapper registrations
                    var autoMapperDescriptors = services.Where(d => 
                        d.ServiceType == typeof(IMapper) || 
                        d.ServiceType == typeof(AutoMapper.IConfigurationProvider)).ToList();

                    foreach (var desc in autoMapperDescriptors)
                    {
                        services.Remove(desc);
                    }

                    // Re-add AutoMapper
                    services.AddAutoMapper(typeof(MappingProfile));
                    _autoMapperInitialized = true;
                }

                // Configure test authentication
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Authentication:Issuer"] = "TestIssuer",
                        ["Authentication:Audience"] = "TestAudience",
                        ["Authentication:SecretForKey"] = "TestSecretKeyForIntegrationTests12345678901234567890"
                    });
                });

                // Create a new service provider
                var serviceProvider = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database context
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<OnlineStoreDBContext>();

                    // Ensure the database is created
                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data
                        TestDataSeeder.SeedTestData(db);
                    }
                    catch (Exception ex)
                    {
                        // Log any errors that occur during seeding
                        Console.WriteLine($"An error occurred seeding the database. Error: {ex.Message}");
                    }
                }
            });
        }
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services => {
                // Add HTTP method override support
                services.AddControllers(options => {
                    options.RespectBrowserAcceptHeader = true;
                });
            });

            return base.CreateHost(builder);
        }
    }
} 