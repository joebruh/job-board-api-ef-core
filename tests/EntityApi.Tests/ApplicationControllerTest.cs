using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using EntityApi.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using EntityApi.API.Models;
using EntityApi.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Slugify;
using Bogus;

namespace EntityApi.Tests;

public class ApplicationControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public ApplicationControllerTest(WebApplicationFactory<Program> factory)
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
    public async Task Candidate_Can_Apply_For_Job()
    {
        await AuthHelper.AuthenticateAsCandidateAsync(_client, "candidate", "secret123");

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

        var payload = new
        {
            Id = jobs[0].Id,
            ResumeLink = "some-link"
        };
        
        var sendApplication = await _client.PostAsJsonAsync($"/api/job-application/{jobs[0].Id}", payload);
        sendApplication.EnsureSuccessStatusCode();

        var applicationExists = await db.Applications.AnyAsync(a =>
            a.JobPostId == payload.Id &&
            a.ResumeLink == payload.ResumeLink
        );

        Assert.True(applicationExists);
    }

    [Fact]
    public async Task Candidate_Can_Get_Application()
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

        var user2 = new User();
        user2.Id = Guid.NewGuid();
        user2.Username = "candidate_username";
        user2.PasswordHash = new PasswordHasher<User>()
            .HashPassword(user2, "sample");
        user2.Role = "Company";

        db.Users.Add(user2);

        var candidate = new CandidateProfile
        {
            Id = Guid.NewGuid(),
            UserId = user2.Id,
            Name = "Candidate Guy",
            Email = "candidate@test.com",
            Phone = "0404040404"
        };

        db.CandidateProfiles.Add(candidate);

        var application = new Application
        {
            JobPostId = jobs[0].Id,
            CandidateProfileId = candidate.Id,
            ResumeLink = "A sample link",
            CreatedAt = DateTime.UtcNow,
        };

        db.Applications.Add(application);

        await db.SaveChangesAsync();

        await AuthHelper.AuthenticateAsCandidateAsync(_client, "candidate_username", "sample");

        var response = await _client.GetAsync($"/api/view-application/{application.Id}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<ViewApplicationDto>();

        Assert.NotNull(content);
        Assert.Equal(application.ResumeLink, content.ResumeLink);
        Assert.Equal(application.Status, content.Status);
    }

}