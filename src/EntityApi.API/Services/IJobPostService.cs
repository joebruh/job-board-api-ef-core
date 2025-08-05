using System;
using EntityApi.Core.Entities;
using EntityApi.API.Models;

namespace EntityApi.API.Services;

public interface IJobPostService
{
    Task<List<JobPostResponseDto>> ListJobs();
    Task<JobPost> PostJob(JobPostDto request);
    Task<JobPost?> UpdateJob(int id, JobPostDto request);
    Task<bool> DeleteJob(int id);
    // Task<TokenResponseDto> RefreshTokensAsync(RefreshTokenRequestDto request);
}
