using System;
using System.Security.Claims;
using EntityApi.API.Models;
using EntityApi.Core.Entities;
using EntityApi.Data;
using Microsoft.EntityFrameworkCore;

namespace EntityApi.API.Services;

public class ApplicationService : IApplicationService
{
    private readonly EntityApiDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILoggedInUserService _loggedInUserService;

    public ApplicationService(EntityApiDbContext context, IHttpContextAccessor httpContextAccessor, ILoggedInUserService loggedInUserService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _loggedInUserService = loggedInUserService;
    }

    public async Task<Application?> SendApplication(int jobPostId, ApplicationSendDto request)
    {
        // Check that job post exists
        var jobPost = await _context.JobPosts
            .Where(j => j.Id == jobPostId)
            .Where(j => j.PublishedAt != null)
            .Where(j => j.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (jobPost is null)
        {
            return null;
        }

        Guid candidateId = (await _loggedInUserService.GetCurrentUserCandidateIdAsync()).Value;

        var application = new Application
        {
            JobPostId = jobPost.Id,
            ResumeLink = request.ResumeLink,
            CandidateProfileId = candidateId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        return application;
    }

    public async Task<ViewApplicationDto?> ViewApplication(int applicationId)
    {
        Guid? candidateProfileId = (await _loggedInUserService.GetCurrentUserCandidateIdAsync()).GetValueOrDefault();
        Guid? companyProfileId = (await _loggedInUserService.GetCurrentUserCompanyIdAsync()).GetValueOrDefault();

        var application = await _context.Applications
            .Include(a => a.JobPost)
            .ThenInclude(j => j.CompanyProfile)
            .Where(a => a.Id == applicationId &&
                (
                    (candidateProfileId != null && a.CandidateProfileId == candidateProfileId) ||
                    (companyProfileId != null && a.JobPost.CompanyProfileId == companyProfileId)
                )
            )
            .FirstOrDefaultAsync();

        if (application is null)
        {
            return null;
        }

        if (companyProfileId is not null && application.Status is null)
        {
            application.Status = "Viewed";
            await _context.SaveChangesAsync();
        }

        var viewApplicationDto = new ViewApplicationDto()
        {
            ResumeLink = application.ResumeLink,
            CoverLetter = application.CoverLetter,
            Status = application.Status,
        };

        return viewApplicationDto;
    }
}
