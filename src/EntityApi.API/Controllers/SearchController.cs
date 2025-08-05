using EntityApi.API.Models;
using EntityApi.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntityApi.API.Controllers;

[Route("api/jobs")]
[ApiController]
public class SearchController(ISearchService searchService) : ControllerBase
{
    [HttpPost("search")]
    public async Task<ActionResult<List<JobPostDto>>> ListJobs(SearchCriteriaDto request)
    {
        return Ok(await searchService.SearchJobs(request));
    }

    [HttpGet("{id}/{slug}")]
    public async Task<ActionResult<List<JobPostDto>>> GetJob(int id, string slug)
    {
        var jobPost = await searchService.GetJobPost(id, slug);

        if (jobPost is null)
        {
            return NotFound();
        }

        return Ok(jobPost);
    }
}
