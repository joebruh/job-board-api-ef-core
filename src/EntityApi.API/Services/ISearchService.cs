using System;
using EntityApi.API.Models;

namespace EntityApi.API.Services;

public interface ISearchService
{
    Task<List<JobPostResponseDto>> SearchJobs(SearchCriteriaDto request);
    Task<SearchResultDto> GetJobPost(int id, string slug);

}
