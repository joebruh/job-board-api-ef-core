using System;

namespace EntityApi.API.Models;

public class ViewApplicationDto
{
    public string ResumeLink { get; set; } = string.Empty;
    public string CoverLetter { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;
}
