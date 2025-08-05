using System.Security.Claims;
using EntityApi.API.Models;
using EntityApi.Core.Entities;
using EntityApi.Data;
using Microsoft.EntityFrameworkCore;

namespace EntityApi.API.Services;

public class JobPostService : IJobPostService
{
    private readonly EntityApiDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILoggedInUserService _loggedInUserService;

    public JobPostService(EntityApiDbContext context, IHttpContextAccessor httpContextAccessor, ILoggedInUserService loggedInUserService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _loggedInUserService = loggedInUserService;
    }

    public async Task<List<JobPostResponseDto>> ListJobs()
    {
        Guid companyProfileId = (await _loggedInUserService.GetCurrentUserCompanyIdAsync()).Value;

        return await _context.JobPosts
            .Where(j => j.DeletedAt == null)
            .Where(j => j.CompanyProfileId == companyProfileId)
            .Select(j => new JobPostResponseDto
            {
                Title = j.Title,
                Description = j.Description,
                PublishedAt = j.PublishedAt,
                UpdatedAt = j.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<JobPost> PostJob(JobPostDto request)
    {

        var jobPost = new JobPost();
        jobPost.CompanyProfileId = (await _loggedInUserService.GetCurrentUserCompanyIdAsync()).Value;
        jobPost.Title = request.Title;
        jobPost.Description = request.Description;
        jobPost.PublishedAt = request.PublishNow ? DateTime.UtcNow : null;
        jobPost.CreatedAt = DateTime.UtcNow;
        jobPost.UpdatedAt = DateTime.UtcNow;

        _context.JobPosts.Add(jobPost);
        await _context.SaveChangesAsync();

        return jobPost;
    }

    public async Task<JobPost?> UpdateJob(int id, JobPostDto request)
    {
        var job = await _context.JobPosts.FindAsync(id);

        if (job is null) {
            return null;
        }

        job.Title = request.Title;
        job.Description = request.Description;
        job.PublishedAt = request.PublishNow ? DateTime.UtcNow : null;
        job.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return job;
    }

    public async Task<bool> DeleteJob(int id)
    {
        var job = await _context.JobPosts.FindAsync(id);

        if (job is null) {
            return false;
        }

        job.DeletedAt = DateTime.UtcNow;
        job.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}
