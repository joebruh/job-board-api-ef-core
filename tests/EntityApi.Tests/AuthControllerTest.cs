using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using EntityApi.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(); // Starts a test server
    }

    private IServiceScope CreateScope() => _factory.Services.CreateScope();

    private EntityApiDbContext GetDbContext(IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<EntityApiDbContext>();
    }

    [Fact]
    public async Task Candidate_Cannot_Register_With_Empty_Fields()
    {
        var payload = new
        {
            Username = "invalid_company",
            Password = "pass",
            Role = "Candidate",
            Email = "",
            Phone = "",
            FullName = "",
        };

        // Register
        var register = await _client.PostAsJsonAsync("/api/Auth/register", payload);
        Assert.Equal(HttpStatusCode.BadRequest, register.StatusCode);

        var content = await register.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.True(content?.Errors.ContainsKey("Email"));
        Assert.Contains("Email is required for candidates.", content.Errors["Email"]);

        Assert.True(content?.Errors.ContainsKey("Phone"));
        Assert.Contains("Phone is required for candidates.", content.Errors["Phone"]);

        Assert.True(content?.Errors.ContainsKey("FullName"));
        Assert.Contains("Full Name is required for candidates.", content.Errors["FullName"]);
    }

    [Fact]
    public async Task Candidate_Can_Register()
    {
        var payload = new
        {
            Username = "testcandidate",
            Password = "pass",
            Role = "Candidate",
            Email = "test@test.com",
            Phone = "040404040",
            FullName = "Test User",
        };

        // Register
        var register = await _client.PostAsJsonAsync("/api/Auth/register", payload);
        register.EnsureSuccessStatusCode();

        using var scope = CreateScope();
        var db = GetDbContext(scope);

        var userExists = await db.Users.AnyAsync(u =>
            u.Username == payload.Username);

        Assert.True(userExists);

        var candidateExists = await db.CandidateProfiles.AnyAsync(c =>
            c.Email == payload.Email &&
            c.Phone == payload.Phone &&
            c.Name == payload.FullName
        );

        Assert.True(candidateExists);
    }

    [Fact]
    public async Task Candidate_Can_Register_And_Login()
    {
        var registerPayload = new
        {
            Username = "testcandidate2",
            Password = "pass",
            Role = "Candidate",
            Email = "test@test.com",
            Phone = "040404040",
            FullName = "Test Candidate",
        };

        // Register
        var register = await _client.PostAsJsonAsync("/api/Auth/register", registerPayload);
        register.EnsureSuccessStatusCode();

        var loginPayload = new
        {
            Username = registerPayload.Username,
            Password = registerPayload.Password,
        };

        // Login
        var login = await _client.PostAsJsonAsync("/api/Auth/login", loginPayload);
        login.EnsureSuccessStatusCode();

        var json = await login.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("accessToken", out _));
        Assert.True(root.TryGetProperty("refreshToken", out _));
    }

    [Fact]
    public async Task Company_Cannot_Register_With_Empty_Fields()
    {
        var payload = new
        {
            Username = "invalid_company",
            Password = "pass",
            Role = "Company",
            CompanyName = "",
            Description = "",
        };

        // Register
        var register = await _client.PostAsJsonAsync("/api/Auth/register", payload);
        Assert.Equal(HttpStatusCode.BadRequest, register.StatusCode);

        var content = await register.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.True(content?.Errors.ContainsKey("CompanyName"));
        Assert.Contains("Company Name is required for companies.", content.Errors["CompanyName"]);

        Assert.True(content?.Errors.ContainsKey("Description"));
        Assert.Contains("Description is required for companies.", content.Errors["Description"]);
    }

    [Fact]
    public async Task Company_Can_Register()
    {
        var payload = new
        {
            Username = "testcompany",
            Password = "pass",
            Role = "Company",
            CompanyName = "Companya",
            Description = "Descriptshon",
        };

        // Register
        var register = await _client.PostAsJsonAsync("/api/Auth/register", payload);
        register.EnsureSuccessStatusCode();

        using var scope = CreateScope();
        var db = GetDbContext(scope);

        var userExists = await db.Users.AnyAsync(u =>
            u.Username == payload.Username);

        Assert.True(userExists);

        var companyExists = await db.CompanyProfiles.AnyAsync(c =>
            c.Name == payload.CompanyName &&
            c.Description == payload.Description
        );

        Assert.True(companyExists);
    }

    [Fact]
    public async Task Company_Can_Register_And_Login()
    {
        var registerPayload = new
        {
            Username = "testcompany2",
            Password = "pass",
            Role = "Company",
            CompanyName = "Companya",
            Description = "Descriptshon",
        };

        // Register
        var register = await _client.PostAsJsonAsync("/api/Auth/register", registerPayload);
        register.EnsureSuccessStatusCode();

        var loginPayload = new
        {
            Username = registerPayload.Username,
            Password = registerPayload.Password,
        };

        // Login
        var login = await _client.PostAsJsonAsync("/api/Auth/login", loginPayload);
        login.EnsureSuccessStatusCode();

        var json = await login.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("accessToken", out _));
        Assert.True(root.TryGetProperty("refreshToken", out _));
    }
}
