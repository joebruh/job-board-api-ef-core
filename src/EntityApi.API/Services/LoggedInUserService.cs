using System;
using System.Security.Claims;
using EntityApi.Data;
using Microsoft.EntityFrameworkCore;

namespace EntityApi.API.Services;

public class LoggedInUserService : ILoggedInUserService
{
    private readonly EntityApiDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public LoggedInUserService(EntityApiDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid?> GetCurrentUserCandidateIdAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = await _context.Users
            .Include(u => u.CandidateProfile)
            .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

        if (user.CandidateProfile is null)
        {
            return null;
        }

        return user.CandidateProfile.Id;
    }

    public async Task<Guid?> GetCurrentUserCompanyIdAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = await _context.Users
            .Include(u => u.CompanyProfile)
            .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

        if (user.CompanyProfile is null)
        {
            return null;
        }

        return user.CompanyProfile.Id;
    }

}
