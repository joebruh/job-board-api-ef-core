using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using EntityApi.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using EntityApi.API.Models;
using EntityApi.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Bogus;
using Slugify;

namespace EntityApi.Tests;

public class SearchControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public SearchControllerTest(WebApplicationFactory<Program> factory)
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
    public async Task Can_Search_For_Jobs()
    {
        // await AuthHelper.AuthenticateAsCompanyAsync(_client, "company", "secret123");

        using var scope = CreateScope();
        var db = GetDbContext(scope);

        var user = new User();
        user.Id = Guid.NewGuid();
        user.Username = "company_username";
        user.PasswordHash = new PasswordHasher<User>()
            .HashPassword(user, "sample");
        user.Role = "Company";

        db.Users.Add(user);

        var company = new CompanyProfile
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = "Acme Corp",
            Description = "Example Company"
        };

        db.CompanyProfiles.Add(company);

        var slugHelper = new SlugHelper();

        var jobs = new Faker<JobPost>()
            .RuleFor(j => j.Title, f => f.Name.JobTitle())
            .RuleFor(j => j.Slug, f => slugHelper.GenerateSlug(f.Name.JobTitle()))
            .RuleFor(j => j.Description, f => f.Lorem.Paragraph())
            .RuleFor(j => j.CompanyProfileId, f => company.Id)
            .RuleFor(j => j.PublishedAt, DateTime.UtcNow)
            .RuleFor(j => j.CreatedAt, DateTime.UtcNow)
            .RuleFor(j => j.UpdatedAt, DateTime.UtcNow)
            .Generate(3);

        db.JobPosts.AddRange(jobs);
        await db.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync("/api/jobs/search", new { });
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<List<JobPostResponseDto>>();
        Assert.Equal(3, data.Count);
    }

    [Fact]
    public async Task Can_Get_Job_Post()
    {

        using var scope = CreateScope();
        var db = GetDbContext(scope);

        var user = new User();
        user.Id = Guid.NewGuid();
        user.Username = "company_username";
        user.PasswordHash = new PasswordHasher<User>()
            .HashPassword(user, "sample");
        user.Role = "Company";

        db.Users.Add(user);

        var company = new CompanyProfile
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = "Acme Corp",
            Description = "Example Company"
        };

        db.CompanyProfiles.Add(company);

        var slugHelper = new SlugHelper();

        var jobs = new Faker<JobPost>()
            .RuleFor(j => j.Title, f => f.Name.JobTitle())
            .RuleFor(j => j.Slug, f => slugHelper.GenerateSlug(f.Name.JobTitle()))
            .RuleFor(j => j.Description, f => f.Lorem.Paragraph())
            .RuleFor(j => j.CompanyProfileId, f => company.Id)
            .RuleFor(j => j.PublishedAt, DateTime.UtcNow)
            .RuleFor(j => j.CreatedAt, DateTime.UtcNow)
            .RuleFor(j => j.UpdatedAt, DateTime.UtcNow)
            .Generate(1);

        db.JobPosts.AddRange(jobs);
        await db.SaveChangesAsync();

        var response = await _client.GetAsync("/api/jobs/1/slug");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<SearchResultDto>();

        Assert.NotNull(content);
        Assert.Equal(jobs[0].Title, content.Title);
        Assert.Equal(jobs[0].Description, content.Description);
        Assert.Equal(jobs[0].Slug, content.slug); // Optional
        Assert.Equal(jobs[0].CompanyProfile.Id, content.CompanyProfile.Id);
    }

}
