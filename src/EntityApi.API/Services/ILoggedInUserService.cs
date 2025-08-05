using System;

namespace EntityApi.API.Services;

public interface ILoggedInUserService
{
    Task<Guid?> GetCurrentUserCandidateIdAsync();
    Task<Guid?> GetCurrentUserCompanyIdAsync();
}
