using System;
using EntityApi.API.Models;
using EntityApi.Core.Entities;

namespace EntityApi.API.Services;

public interface IApplicationService
{
    public Task<Application> SendApplication(int jobPostId, ApplicationSendDto request);
    public Task<ViewApplicationDto?> ViewApplication(int applicationId);
}
