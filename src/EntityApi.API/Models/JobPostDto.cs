using System;

namespace EntityApi.API.Models;

public class JobPostDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool PublishNow { get; set; } = false;
}
