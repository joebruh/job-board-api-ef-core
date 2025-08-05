using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace EntityApi.Tests;

public static class AuthHelper
{
    public static async Task AuthenticateAsCandidateAsync(HttpClient client, string username, string password)
    {
        // Register (ignore failure if user already exists)
        var register = await client.PostAsJsonAsync("/api/Auth/register", new
        {
            Username = username,
            Password = password,
            Role = "Candidate",
            Email = $"{username}@test.com",
            Phone = "123456789",
            FullName = "Test User"
        });

        if (!register.IsSuccessStatusCode && register.StatusCode != HttpStatusCode.BadRequest)
            throw new Exception("Failed to register test user");

        // Login
        var login = await client.PostAsJsonAsync("/api/Auth/login", new
        {
            Username = username,
            Password = password
        });

        login.EnsureSuccessStatusCode();

        var json = await login.Content.ReadFromJsonAsync<JsonElement>();
        var token = json.GetProperty("token").GetString();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    public static async Task AuthenticateAsCompanyAsync(HttpClient client, string username, string password)
    {
        // Register (ignore failure if user already exists)
        var register = await client.PostAsJsonAsync("/api/Auth/register", new
        {
            Username = username,
            Password = password,
            Role = "Company",
            CompanyName = "Company Test",
            Description = "Description"
        });

        if (!register.IsSuccessStatusCode && register.StatusCode != HttpStatusCode.BadRequest)
            throw new Exception("Failed to register test user");

        // Login
        var login = await client.PostAsJsonAsync("/api/Auth/login", new
        {
            Username = username,
            Password = password
        });

        login.EnsureSuccessStatusCode();

        var json = await login.Content.ReadFromJsonAsync<JsonElement>();
        var token = json.GetProperty("accessToken").GetString();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
