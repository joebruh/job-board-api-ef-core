using EntityApi.API.Models;
using EntityApi.API.Services;
using EntityApi.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EntityApi.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApplicationController(IApplicationService applicationService) : ControllerBase
{
    [Authorize(Roles = "Candidate")]
    [HttpPost("/api/job-application/{JobPostId}")]
    public async Task<ActionResult<Application>> SendApplication(int JobPostId, ApplicationSendDto request)
    {
        var application = await applicationService.SendApplication(JobPostId, request);

        return application;
    }

    [Authorize(Roles = "Candidate,Company")]
    [HttpGet("/api/view-application/{ApplicationId}")]
    public async Task<ActionResult<ViewApplicationDto>?> ViewApplication(int ApplicationId)
    {
        var application = await applicationService.ViewApplication(ApplicationId);

        if (application is null)
        {
            return NotFound();
        }

        return application;
    }


}

