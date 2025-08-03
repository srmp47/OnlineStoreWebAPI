using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineStoreWebAPI.DBContext;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace TestProject.Helper
{
    public abstract class RepositoryTestBase
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly Mock<ILogger> MockLogger;

        protected RepositoryTestBase()
        {
            var services = new ServiceCollection();
            
            // Setup in-memory database
            services.AddDbContext<OnlineStoreDBContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
            
            // Setup logging
            MockLogger = new Mock<ILogger>();
            services.AddLogging(builder => builder.AddConsole());
            
            ServiceProvider = services.BuildServiceProvider();
        }

        protected T GetService<T>() where T : class
        {
            return ServiceProvider.GetRequiredService<T>();
        }

        protected OnlineStoreDBContext GetDbContext()
        {
            return ServiceProvider.GetRequiredService<OnlineStoreDBContext>();
        }

        protected void AssertSuccess<T>(T result) where T : class
        {
            result.Should().NotBeNull();
        }

        protected void AssertFailure<T>(T result) where T : class
        {
            result.Should().BeNull();
        }
    }
} 