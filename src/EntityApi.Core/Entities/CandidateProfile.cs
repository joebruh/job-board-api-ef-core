using System;

namespace EntityApi.Core.Entities;

public class CandidateProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Guid UserId { get; set; }

    // Navigation
    public User User { get; set; } = null!;

    // public ICollection<Application> Applications { get; set; } = new List<Application>();
}
