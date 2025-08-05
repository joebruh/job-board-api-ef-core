using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using EntityApi.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using EntityApi.API.Models;

namespace EntityApi.Tests;

public class JobPostControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public JobPostControllerTest(WebApplicationFactory<Program> factory)
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
    public async Task Company_Can_Post_Job()
    {
        await AuthHelper.AuthenticateAsCompanyAsync(_client, "user1", "secret123");

        var payload = new
        {
            Title = "New Job Test",
            Description = "This is a job description",
            PublishNow = false
        };

        var postJob = await _client.PostAsJsonAsync("/api/JobPost", payload);
        postJob.EnsureSuccessStatusCode();

        using var scope = CreateScope();
        var db = GetDbContext(scope);

        var jobPostExists = await db.JobPosts.AnyAsync(j =>
            j.Title == payload.Title &&
            j.Description == payload.Description
        );

        Assert.True(jobPostExists);
    }

    [Fact]
    public async Task Company_Can_List_Their_Jobs()
    {
        await AuthHelper.AuthenticateAsCompanyAsync(_client, "company", "secret123");

        // TODO generate job posts without using endpoints

        // 1
        await _client.PostAsJsonAsync("/api/JobPost", new
        {
            Title = "A Job Test",
            Description = "This is a job description",
            PublishNow = false
        });

        // 2
        await _client.PostAsJsonAsync("/api/JobPost", new
        {
            Title = "Another Job Test",
            Description = "This is a job description",
            PublishNow = false
        });

        var response = await _client.GetAsync("/api/JobPost");

        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<List<JobPostResponseDto>>();
        Assert.Equal(2, data.Count);
    }

    [Fact]
    public async Task Company_Can_Update_Job()
    {
        await AuthHelper.AuthenticateAsCompanyAsync(_client, "user1", "secret123");

        var payload = new
        {
            Title = "Now the updated job",
            Description = "Updated job description",
        };

        var postJob = await _client.PutAsJsonAsync("/api/JobPost/1", payload);
        postJob.EnsureSuccessStatusCode();

        using var scope = CreateScope();
        var db = GetDbContext(scope);

        var jobPostExists = await db.JobPosts.AnyAsync(j =>
            j.Id == 1 &&
            j.Title == payload.Title &&
            j.Description == payload.Description
        );

        Assert.True(jobPostExists);
    }

    [Fact]
    public async Task Company_Can_Delete_Job()
    {
        await AuthHelper.AuthenticateAsCompanyAsync(_client, "user1", "secret123");

        var postJob = await _client.DeleteAsync("/api/JobPost/1");
        postJob.EnsureSuccessStatusCode();

        using var scope = CreateScope();
        var db = GetDbContext(scope);

        var jobPostExists = await db.JobPosts.AnyAsync(j =>
            j.Id == 1 &&
            j.DeletedAt != null
        );

        Assert.True(jobPostExists);
    }

}
