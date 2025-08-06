using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineStoreWebAPI.DBContext;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TestProject.Integration;

namespace TestProject.Integration
{
    public abstract class BaseIntegrationTest : IClassFixture<TestWebApplicationFactory<Program>>
    {
        protected readonly TestWebApplicationFactory<Program> _factory;
        protected readonly HttpClient _client;
        protected readonly OnlineStoreDBContext _context;

        protected BaseIntegrationTest(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            // Add default authorization header for tests
            _client.DefaultRequestHeaders.Add("Authorization", TestAuthHelper.GetTestAuthorizationHeader());

            // Get the database context for test setup/cleanup
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<OnlineStoreDBContext>();
        }

        protected async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _client.GetAsync(url);
        }

        protected async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PostAsync(url, content);
        }

        protected async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PutAsync(url, content);
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await _client.DeleteAsync(url);
        }

        protected async Task<T?> GetFromJsonAsync<T>(string url)
        {
            return await _client.GetFromJsonAsync<T>(url);
        }

        protected async Task<T?> GetFromJsonAsync<T>(HttpResponseMessage response)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        protected void ResetDatabase()
        {
            TestDataSeeder.ClearTestData(_context);
            TestDataSeeder.SeedTestData(_context);
        }

        protected void ClearDatabase()
        {
            TestDataSeeder.ClearTestData(_context);
        }
        protected void ClearOrderOfUser(int userId)
        {
            var orders = _context.Orders.Where(o => o.userId == userId).ToList();
            _context.Orders.RemoveRange(orders);
            _context.SaveChanges();
        }

    }
} 