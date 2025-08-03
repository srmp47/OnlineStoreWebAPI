
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


// TODO implement microservices architecture 
// TODO specification design pattern
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OnlineStoreDBContext>(option =>
{
    option.UseSqlite(
        builder.Configuration["ConnectionStrings:CityConnectionString"]
        );
});
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository,ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();



builder.Services.AddAutoMapper(typeof(MappingProfile));
// Configure JSON serialization to preserve object references and pretty-print the output
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
        };
    }
    );

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin")); 

    
    options.AddPolicy("Customer", policy =>
        policy.RequireRole("Customer"));
});
// Register GraphQL services
builder.Services
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
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderItemService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<OrderItemRepository>();
builder.Services.AddScoped<ProductRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapGraphQL("/graphql");

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
