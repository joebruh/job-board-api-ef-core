using EntityApi.Core.Entities;
using EntityApi.API.Models;
using EntityApi.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntityApi.API.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "Company")]
[ApiController]
public class JobPostController(IJobPostService jobPostService) : ControllerBase
{
    [HttpPost("post")]
    public async Task<ActionResult<JobPost>> PostJob(JobPostDto request)
    {
        var jobPost = await jobPostService.PostJob(request);

        return Ok(jobPost);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<JobPost>> UpdateJob(int id, JobPostDto request)
    {
        var jobPost = await jobPostService.UpdateJob(id, request);

        return Ok(jobPost);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteJob(int id)
    {
        var jobPostStatus = await jobPostService.DeleteJob(id);

        return Ok(jobPostStatus);
    }
}
