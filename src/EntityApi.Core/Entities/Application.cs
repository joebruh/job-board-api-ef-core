using System;

namespace EntityApi.Core.Entities;

public class Application
{
    public int Id { get; set; }
    public Guid CandidateProfileId { get; set; }
    public int JobPostId { get; set; }
    public string ResumeLink { get; set; } = string.Empty;
    public string CoverLetter { get; set; } = string.Empty;
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }


    public CandidateProfile CandidateProfile { get; set; } = default!;
    public JobPost JobPost { get; set; } = default!;
}
