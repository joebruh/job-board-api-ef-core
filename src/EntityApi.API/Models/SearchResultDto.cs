using System;
using EntityApi.Core.Entities;

namespace EntityApi.API.Models;

public class SearchResultDto
{
    public string Title { get; set; } = string.Empty;
    public string slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public CompanyProfile CompanyProfile {get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
