using System;
using EntityApi.Core.Entities;

namespace EntityApi.API.Models;

public class JobPostResponseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
