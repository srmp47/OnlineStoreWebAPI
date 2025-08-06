using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Mapping;
using OnlineStoreWebAPI.Repository;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OnlineStoreWebAPI.GraphQL;
using HotChocolate;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace TestProject.Integration
{
    public class TestStartup
    {
        public IConfiguration Configuration { get; }

        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            
            services.AddDbContext<OnlineStoreDBContext>(option =>
            {
                option.UseInMemoryDatabase("TestDatabase");
            });
            
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();

            // Configure AutoMapper for tests
            services.AddAutoMapper(typeof(MappingProfile));

            // Configure JSON serialization
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Authentication:Issuer"],
                        ValidAudience = Configuration["Authentication:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(Configuration["Authentication:SecretForKey"] ?? "DefaultSecretKey"))
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("Customer", policy =>
                    policy.RequireRole("Customer"));
            });

            // Register GraphQL services
            services
                .AddGraphQLServer()
                .AddAuthorization()
                .AddQueryType<UserQuery>()
                .AddMutationType<UserMutation>()
                .AddTypeExtension<ProductQuery>()
                .AddTypeExtension<ProductMutation>()
                .AddTypeExtension<OrderQuery>()
                .AddTypeExtension<OrderMutation>()
                .AddTypeExtension<OrderItemQuery>()
                .AddTypeExtension<OrderItemMutation>();

            // Register repositories for direct injection (needed for GraphQL [Service] injection)
            services.AddScoped<UserService>();
            services.AddScoped<OrderService>();
            services.AddScoped<OrderItemService>();
            services.AddScoped<ProductService>();
            services.AddScoped<UserRepository>();
            services.AddScoped<OrderRepository>();
            services.AddScoped<OrderItemRepository>();
            services.AddScoped<ProductRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseAuthorization();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL("/graphql");
            });
        }
    }
} 