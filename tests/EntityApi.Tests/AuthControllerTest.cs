using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using EntityApi.API; // Your main API project namespace
using Microsoft.AspNetCore.Mvc.Testing;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(); // Starts a test server
    }

    [Fact]
    public async Task Candiate_Can_Register()
    {
        // Register
        var register = await _client.PostAsJsonAsync("/api/Auth/register", new
        {
            Username = "testuser",
            Password = "pass",
            Role = "Candidate",
            Email = "test@test.com",
            phone = "040404040",
            fullName = "Test User",
        });
        register.EnsureSuccessStatusCode();
    }
}
