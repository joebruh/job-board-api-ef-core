using System;
using EntityApi.API.Models;
using EntityApi.Core.Entities;
using EntityApi.Data;
using Microsoft.EntityFrameworkCore;

namespace EntityApi.API.Services;

public class SearchService : ISearchService
{
    private readonly EntityApiDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public SearchService(EntityApiDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<JobPostResponseDto>> SearchJobs(SearchCriteriaDto request)
    {
        return await _context.JobPosts
            .Where(j => j.DeletedAt == null)
            .Where(j => j.PublishedAt != null)
            .Select(j => new JobPostResponseDto
            {
                Title = j.Title,
                Description = j.Description,
                PublishedAt = j.PublishedAt,
                UpdatedAt = j.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<SearchResultDto?> GetJobPost(int id, string slug)
    {
        var jobPost = await _context.JobPosts
            .Where(j => j.Id == id)
            .Where(j => j.DeletedAt == null)
            .Where(j => j.PublishedAt != null)
            .Select(j => new SearchResultDto
            {
                Title = j.Title,
                slug = j.Slug,
                Description = j.Description,
                CompanyProfile = j.CompanyProfile,
                PublishedAt = j.PublishedAt,
                UpdatedAt = j.UpdatedAt
            })
            .SingleOrDefaultAsync();

        return jobPost;

    }
}
